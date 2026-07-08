using BIRPOSSystem.Data;
using BIRPOSSystem.Models;
using BIRPOSSystem.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/reports")]
public sealed class BIRReportController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet("z-reading/today")]
    public async Task<ZReadingDto> GetTodayZReadingAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var start = today.ToDateTime(TimeOnly.MinValue);
        var end = today.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var branch = await db.Branches.OrderBy(x => x.CreatedAt).FirstAsync();
        var terminal = await db.PosTerminals.Where(x => x.IsActive).OrderBy(x => x.CreatedAt).FirstAsync();

        var sales = await db.SalesTransactions
            .Where(x => x.TerminalId == terminal.Id && x.SoldAt >= start && x.SoldAt < end)
            .OrderBy(x => x.SoldAt)
            .ToListAsync();

        var completed = sales.Where(x => x.Status == SaleStatus.Completed).ToList();

        return new ZReadingDto(
            branch.Name,
            terminal.Code,
            today,
            sales.FirstOrDefault()?.InvoiceNumber ?? "N/A",
            sales.LastOrDefault()?.InvoiceNumber ?? "N/A",
            completed.Count,
            completed.Sum(x => x.GrossTotal),
            completed.Sum(x => x.DiscountTotal),
            completed.Sum(x => x.VatSales),
            completed.Sum(x => x.VatAmount),
            completed.Sum(x => x.NetTotal),
            sales.Where(x => x.Status == SaleStatus.Voided).Sum(x => x.NetTotal),
            sales.Where(x => x.Status == SaleStatus.Refunded).Sum(x => x.NetTotal),
            null);
    }
}
