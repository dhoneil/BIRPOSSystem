using BIRPOSSystem.Models.Common;

namespace BIRPOSSystem.Models.Sync;

public sealed class SyncOutboxItem : EntityBase
{
    public DateTime QueuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UploadedAt { get; set; }
    public SyncItemStatus Status { get; set; } = SyncItemStatus.Pending;
    public string ItemType { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public string LastError { get; set; } = string.Empty;
}
