using BIRPOSSystem.Data;
using BIRPOSSystem.Models;
using BIRPOSSystem.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<DashboardDto> GetDashboardAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var start = today.ToDateTime(TimeOnly.MinValue);
        var end = today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var sales = await db.SalesTransactions
            .Where(x => x.SoldAt >= start && x.SoldAt < end)
            .OrderByDescending(x => x.SoldAt)
            .ToListAsync();

        var shift = await db.CashShifts
            .Where(x => x.Status == ShiftStatus.Open)
            .OrderByDescending(x => x.OpenedAt)
            .FirstOrDefaultAsync();

        var tenant = await db.Tenants.FirstAsync();
        var pendingSync = await db.SyncOutboxItems.CountAsync(x => x.Status == SyncItemStatus.Pending);
        var lastSync = await db.SyncOutboxItems
            .Where(x => x.UploadedAt != null)
            .OrderByDescending(x => x.UploadedAt)
            .Select(x => x.UploadedAt!.Value)
            .FirstOrDefaultAsync();

        var cashPayments = await db.SalesPayments
            .Where(x => x.Method == "Cash" && x.SalesTransaction!.SoldAt >= start && x.SalesTransaction.SoldAt < end)
            .SumAsync(x => (decimal?)x.Amount) ?? 0;

        var alerts = await db.Products
            .Where(x => x.IsActive && x.TrackInventory && x.QuantityOnHand <= x.ReorderPoint)
            .OrderBy(x => x.QuantityOnHand)
            .Take(6)
            .Select(x => new InventoryAlertDto(x.Sku, x.Name, x.QuantityOnHand, x.ReorderPoint))
            .ToListAsync();

        var recentSales = sales.Take(8)
            .Select(x => new RecentSaleDto(x.InvoiceNumber, x.SoldAt, x.CashierName, x.NetTotal, x.Status.ToString()))
            .ToList();

        var summary = new DashboardSummaryDto(
            sales.Where(x => x.Status == SaleStatus.Completed).Sum(x => x.NetTotal),
            sales.Count(x => x.Status == SaleStatus.Completed),
            (shift?.OpeningCash ?? 0) + cashPayments,
            pendingSync,
            shift is null ? "No open shift" : $"{shift.CashierName} opened {shift.OpenedAt:hh:mm tt}",
            $"{tenant.SubscriptionStatus} until {tenant.SubscriptionValidUntil:MMM d, yyyy}",
            lastSync == default ? DateTime.MinValue : lastSync);

        return new DashboardDto(summary, recentSales, alerts);
    }
}
