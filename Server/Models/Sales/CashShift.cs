using BIRPOSSystem.Models.Common;
using BIRPOSSystem.Models.Tenancy;

namespace BIRPOSSystem.Models.Sales;

public sealed class CashShift : EntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid TerminalId { get; set; }
    public PosTerminal? Terminal { get; set; }
    public string CashierName { get; set; } = string.Empty;
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public decimal OpeningCash { get; set; }
    public decimal ClosingCash { get; set; }
    public ShiftStatus Status { get; set; } = ShiftStatus.Open;
}
