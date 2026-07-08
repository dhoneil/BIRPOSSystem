using BIRPOSSystem.Data;
using BIRPOSSystem.Models;
using BIRPOSSystem.Shared;
using BIRPOSSystem.Shared.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/pos")]
public sealed class POSController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet("catalog")]
    public async Task<PosCatalogDto> GetCatalogAsync()
    {
        var categories = await db.ProductCategories
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new ProductCategoryDto(x.Id, x.Name))
            .ToListAsync();

        var products = await db.Products
            .Include(x => x.Category)
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsCafeItem)
            .ThenBy(x => x.Category!.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new ProductDto(
                x.Id,
                x.Sku,
                x.Name,
                x.Category!.Name,
                x.Price,
                x.QuantityOnHand,
                x.IsVatExempt,
                x.IsActive))
            .ToListAsync();

        var branch = await db.Branches.OrderBy(x => x.CreatedAt).FirstAsync();
        var terminal = await db.PosTerminals.Where(x => x.IsActive).OrderBy(x => x.CreatedAt).FirstAsync();
        var series = await db.ReceiptSeries.Where(x => x.IsActive && x.TerminalId == terminal.Id).FirstAsync();
        var shift = await db.CashShifts.Where(x => x.Status == ShiftStatus.Open).OrderByDescending(x => x.OpenedAt).FirstOrDefaultAsync();

        var state = new PosRegisterStateDto(
            branch.Name,
            terminal.Code,
            series.Prefix,
            series.PeekNextInvoiceNumber(),
            shift?.CashierName ?? "No open shift",
            SaleCalculator.DefaultVatRate);

        return new PosCatalogDto(categories, products, state);
    }
}
