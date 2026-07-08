using BIRPOSSystem.Models.Common;
using BIRPOSSystem.Models.Tenancy;

namespace BIRPOSSystem.Models.Sales;

public sealed class SalesTransaction : EntityBase
{
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Guid TerminalId { get; set; }
    public PosTerminal? Terminal { get; set; }
    public Guid? ShiftId { get; set; }
    public CashShift? Shift { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SoldAt { get; set; } = DateTime.UtcNow;
    public string CashierName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string OrderType { get; set; } = "Retail";
    public decimal GrossTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal VatSales { get; set; }
    public decimal VatAmount { get; set; }
    public decimal VatExemptSales { get; set; }
    public decimal NetTotal { get; set; }
    public decimal ChangeDue { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Completed;
    public string Notes { get; set; } = string.Empty;
    public ICollection<SalesLine> Lines { get; set; } = [];
    public ICollection<SalesPayment> Payments { get; set; } = [];
}
