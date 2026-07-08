using BIRPOSSystem.Models.Common;

namespace BIRPOSSystem.Models.Compliance;

public sealed class AuditLog : EntityBase
{
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
}
