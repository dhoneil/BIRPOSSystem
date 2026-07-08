using BIRPOSSystem.Models.Common;
using BIRPOSSystem.Models.Tenancy;

namespace BIRPOSSystem.Models.Sales;

public sealed class ZReading : EntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid TerminalId { get; set; }
    public PosTerminal? Terminal { get; set; }
    public DateOnly BusinessDate { get; set; }
    public string BeginningInvoiceNumber { get; set; } = string.Empty;
    public string EndingInvoiceNumber { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal GrossSales { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal VatSales { get; set; }
    public decimal VatAmount { get; set; }
    public decimal NetSales { get; set; }
    public decimal VoidTotal { get; set; }
    public decimal RefundTotal { get; set; }
    public DateTime? ClosedAt { get; set; }
}
