using BIRPOSSystem.Models.Common;
using BIRPOSSystem.Models.Sales;

namespace BIRPOSSystem.Models.Tenancy;

public sealed class PosTerminal : EntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public string Code { get; set; } = string.Empty;
    public string MachineIdentificationNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string PermitNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<ReceiptSeries> ReceiptSeries { get; set; } = [];
}
