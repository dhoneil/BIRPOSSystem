using BIRPOSSystem.Models.Common;

namespace BIRPOSSystem.Models.Tenancy;

public sealed class Tenant : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public string Tin { get; set; } = string.Empty;
    public BusinessType PrimaryBusinessType { get; set; } = BusinessType.Retail;
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;
    public DateOnly SubscriptionValidUntil { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
    public int OfflineGraceDays { get; set; } = 14;
    public ICollection<Branch> Branches { get; set; } = [];
}
