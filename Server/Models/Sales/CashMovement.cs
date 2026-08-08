using BIRPOSSystem.Models.Common;
using BIRPOSSystem.Models.Tenancy;

namespace BIRPOSSystem.Models.Sales;

public sealed class CashMovement : EntityBase
{
    public Guid CashShiftId { get; set; }
    public CashShift? CashShift { get; set; }
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid TerminalId { get; set; }
    public PosTerminal? Terminal { get; set; }
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    public CashMovementType Type { get; set; } = CashMovementType.CashIn;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
}
