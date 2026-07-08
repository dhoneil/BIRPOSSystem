using BIRPOSSystem.Models.Catalog;
using BIRPOSSystem.Models.Common;

namespace BIRPOSSystem.Models.Sales;

public sealed class SalesLine : EntityBase
{
    public Guid SalesTransactionId { get; set; }
    public SalesTransaction? SalesTransaction { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VatAmount { get; set; }
    public decimal NetAmount { get; set; }
    public bool IsVatExempt { get; set; }
}
