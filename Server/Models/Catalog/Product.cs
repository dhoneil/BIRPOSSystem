using BIRPOSSystem.Models.Common;

namespace BIRPOSSystem.Models.Catalog;

public sealed class Product : EntityBase
{
    public Guid CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal ReorderPoint { get; set; }
    public bool TrackInventory { get; set; } = true;
    public bool IsVatExempt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsCafeItem { get; set; }
}
