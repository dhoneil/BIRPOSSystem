using BIRPOSSystem.Models.Catalog;
using BIRPOSSystem.Models.Common;

namespace BIRPOSSystem.Models.Inventory;

public sealed class InventoryLedgerEntry : EntityBase
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    public string ReferenceType { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal QuantityIn { get; set; }
    public decimal QuantityOut { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Remarks { get; set; } = string.Empty;
}
