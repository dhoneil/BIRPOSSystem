using BIRPOSSystem.Models.Common;

namespace BIRPOSSystem.Models.Sales;

public sealed class SalesPayment : EntityBase
{
    public Guid SalesTransactionId { get; set; }
    public SalesTransaction? SalesTransaction { get; set; }
    public string Method { get; set; } = "Cash";
    public decimal Amount { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
}
