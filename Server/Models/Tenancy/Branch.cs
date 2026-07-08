using BIRPOSSystem.Models.Common;

namespace BIRPOSSystem.Models.Tenancy;

public sealed class Branch : EntityBase
{
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string BirPermitNumber { get; set; } = string.Empty;
    public string BirAccreditationNumber { get; set; } = string.Empty;
    public ICollection<PosTerminal> Terminals { get; set; } = [];
}
