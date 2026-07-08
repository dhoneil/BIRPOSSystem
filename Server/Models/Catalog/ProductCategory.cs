using BIRPOSSystem.Models.Common;

namespace BIRPOSSystem.Models.Catalog;

public sealed class ProductCategory : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string DisplayColor { get; set; } = "#164E63";
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Product> Products { get; set; } = [];
}
