namespace BIRPOSSystem.Shared;

public sealed record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string CategoryName,
    decimal Price,
    decimal QuantityOnHand,
    bool IsVatExempt,
    bool IsActive);

public sealed record ProductCategoryDto(Guid Id, string Name);

public sealed record UpsertProductRequest(
    string Sku,
    string Name,
    Guid CategoryId,
    decimal Price,
    decimal QuantityOnHand,
    decimal ReorderPoint,
    bool IsVatExempt,
    bool IsActive);
