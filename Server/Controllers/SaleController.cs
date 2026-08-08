using System.Text.Json;
using BIRPOSSystem.Data;
using BIRPOSSystem.Models;
using BIRPOSSystem.Models.Compliance;
using BIRPOSSystem.Models.Inventory;
using BIRPOSSystem.Models.Sales;
using BIRPOSSystem.Models.Sync;
using BIRPOSSystem.Shared;
using BIRPOSSystem.Shared.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/sales")]
public sealed class SaleController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet("history")]
    public async Task<IReadOnlyList<SalesHistoryDto>> GetHistoryAsync()
    {
        var sales = await db.SalesTransactions
            .AsNoTracking()
            .OrderByDescending(x => x.SoldAt)
            .Take(100)
            .Select(x => new
            {
                x.Id,
                x.InvoiceNumber,
                x.SoldAt,
                x.CustomerName,
                x.CashierName,
                x.Status,
                x.NetTotal,
                x.OrderType,
                ItemCount = x.Lines.Count
            })
            .ToListAsync();

        return sales
            .Select(x => new SalesHistoryDto(
                x.Id,
                x.InvoiceNumber,
                x.SoldAt,
                string.IsNullOrWhiteSpace(x.CustomerName) ? "Walk-in" : x.CustomerName,
                x.CashierName,
                x.Status.ToString(),
                x.NetTotal,
                x.OrderType,
                x.ItemCount))
            .ToList();
    }

    [HttpPost]
    public async Task<IActionResult> CreateSaleAsync([FromBody] CreateSaleRequest request)
    {
        if (request.Lines.Count == 0)
        {
            return BadRequest("Cart is empty.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync();

        var tenant = await db.Tenants.OrderBy(x => x.CreatedAt).FirstAsync();
        var branch = await db.Branches.OrderBy(x => x.CreatedAt).FirstAsync();
        var terminal = await db.PosTerminals.Where(x => x.IsActive).OrderBy(x => x.CreatedAt).FirstAsync();
        var series = await db.ReceiptSeries.Where(x => x.IsActive && x.TerminalId == terminal.Id).FirstAsync();
        var shift = await db.CashShifts.Where(x => x.Status == ShiftStatus.Open).OrderByDescending(x => x.OpenedAt).FirstOrDefaultAsync();
        if (shift is null)
        {
            return BadRequest("Open a cash shift before taking sales.");
        }

        var productIds = request.Lines.Select(x => x.ProductId).Distinct().ToArray();
        var products = await db.Products.Where(x => productIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var missingProduct = productIds.FirstOrDefault(id => !products.ContainsKey(id));
        if (missingProduct != Guid.Empty)
        {
            return BadRequest("One or more cart products no longer exist.");
        }

        var calculator = new SaleCalculator();
        SaleCalculationResult calculation;
        try
        {
            calculation = calculator.Calculate(
                request.Lines.Select(line =>
                {
                    var product = products[line.ProductId];
                    return new SaleCalculationLine(
                        product.Id,
                        product.Sku,
                        product.Name,
                        line.Quantity,
                        line.UnitPrice,
                        line.DiscountAmount,
                        product.IsVatExempt);
                }),
                request.ManualDiscountAmount);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        var paid = request.Payments.Sum(x => x.Amount);
        if (paid < calculation.NetTotal)
        {
            return BadRequest("Payment is less than the sale total.");
        }

        var invoiceNumber = series.ConsumeNextInvoiceNumber();
        var sale = new SalesTransaction
        {
            TenantId = tenant.Id,
            BranchId = branch.Id,
            TerminalId = terminal.Id,
            ShiftId = shift.Id,
            InvoiceNumber = invoiceNumber,
            CashierName = shift.CashierName,
            CustomerName = request.CustomerName?.Trim() ?? string.Empty,
            OrderType = string.IsNullOrWhiteSpace(request.OrderType) ? "Retail" : request.OrderType,
            GrossTotal = calculation.GrossTotal,
            DiscountTotal = calculation.DiscountTotal,
            VatSales = calculation.VatSales,
            VatAmount = calculation.VatAmount,
            VatExemptSales = calculation.VatExemptSales,
            NetTotal = calculation.NetTotal,
            ChangeDue = paid - calculation.NetTotal,
            Notes = request.Notes?.Trim() ?? string.Empty,
            Lines = calculation.Lines.Select(line => new SalesLine
            {
                ProductId = line.ProductId,
                Sku = line.Sku,
                ProductName = line.ProductName,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                GrossAmount = line.GrossAmount,
                DiscountAmount = line.DiscountAmount,
                VatAmount = line.VatAmount,
                NetAmount = line.NetAmount,
                IsVatExempt = line.IsVatExempt
            }).ToList(),
            Payments = request.Payments.Select(payment => new SalesPayment
            {
                Method = payment.Method,
                Amount = payment.Amount,
                ReferenceNumber = payment.ReferenceNumber ?? string.Empty
            }).ToList()
        };

        foreach (var line in calculation.Lines)
        {
            var product = products[line.ProductId];
            if (!product.TrackInventory)
            {
                continue;
            }

            product.QuantityOnHand -= line.Quantity;
            db.InventoryLedgerEntries.Add(new InventoryLedgerEntry
            {
                ProductId = product.Id,
                ReferenceType = "Sale",
                ReferenceNumber = invoiceNumber,
                QuantityOut = line.Quantity,
                BalanceAfter = product.QuantityOnHand,
                Remarks = $"Sold via {terminal.Code}"
            });
        }

        db.SalesTransactions.Add(sale);
        db.AuditLogs.Add(new AuditLog
        {
            UserName = sale.CashierName,
            Action = "SaleCompleted",
            EntityName = nameof(SalesTransaction),
            EntityId = sale.Id.ToString(),
            Details = $"{invoiceNumber} total {sale.NetTotal:N2}"
        });
        db.SyncOutboxItems.Add(new SyncOutboxItem
        {
            ItemType = nameof(SalesTransaction),
            ItemId = sale.Id.ToString(),
            PayloadJson = JsonSerializer.Serialize(new
            {
                sale.Id,
                sale.InvoiceNumber,
                sale.SoldAt,
                sale.NetTotal,
                sale.TerminalId
            })
        });

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new CreateSaleResponse(
            sale.Id,
            sale.InvoiceNumber,
            sale.GrossTotal,
            sale.DiscountTotal,
            sale.VatAmount,
            sale.NetTotal,
            sale.ChangeDue,
            sale.SoldAt));
    }
}
