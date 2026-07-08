using BIRPOSSystem.Data;
using BIRPOSSystem.Models;
using BIRPOSSystem.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/sync")]
public sealed class SyncController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet("status")]
    public async Task<SyncStatusDto> GetStatusAsync()
    {
        var tenant = await db.Tenants.FirstAsync();
        var pending = await db.SyncOutboxItems.CountAsync(x => x.Status == SyncItemStatus.Pending);
        var lastSync = await db.SyncOutboxItems
            .Where(x => x.UploadedAt != null)
            .OrderByDescending(x => x.UploadedAt)
            .Select(x => x.UploadedAt!.Value)
            .FirstOrDefaultAsync();

        return new SyncStatusDto(
            true,
            pending,
            lastSync == default ? DateTime.MinValue : lastSync,
            pending == 0 ? "Local data is fully uploaded." : $"{pending} local item(s) waiting for upload.",
            tenant.SubscriptionStatus.ToString(),
            tenant.SubscriptionValidUntil);
    }

    [HttpPost("upload")]
    public async Task<SyncUploadResponse> UploadAsync()
    {
        var pending = await db.SyncOutboxItems
            .Where(x => x.Status == SyncItemStatus.Pending)
            .OrderBy(x => x.QueuedAt)
            .ToListAsync();

        var now = DateTime.UtcNow;
        foreach (var item in pending)
        {
            item.Status = SyncItemStatus.Uploaded;
            item.UploadedAt = now;
            item.LastError = string.Empty;
        }

        await db.SaveChangesAsync();

        return new SyncUploadResponse(
            pending.Count,
            now,
            pending.Count == 0 ? "Nothing to upload." : "Pending local records were marked as uploaded.");
    }
}
