namespace BIRPOSSystem.Shared.Sales;

public sealed record SaleCalculationLine(
    Guid ProductId,
    string Sku,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountAmount,
    bool IsVatExempt);

public sealed record CalculatedSaleLine(
    Guid ProductId,
    string Sku,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice,
    decimal GrossAmount,
    decimal DiscountAmount,
    decimal VatAmount,
    decimal NetAmount,
    bool IsVatExempt);

public sealed record SaleCalculationResult(
    IReadOnlyList<CalculatedSaleLine> Lines,
    decimal GrossTotal,
    decimal DiscountTotal,
    decimal VatSales,
    decimal VatAmount,
    decimal VatExemptSales,
    decimal NetTotal);
