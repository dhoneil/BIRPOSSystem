namespace BIRPOSSystem.Shared;

public sealed record PosCatalogDto(
    IReadOnlyList<ProductCategoryDto> Categories,
    IReadOnlyList<ProductDto> Products,
    PosRegisterStateDto RegisterState);

public sealed record PosRegisterStateDto(
    string BranchName,
    string TerminalCode,
    string ReceiptSeries,
    string NextInvoiceNumber,
    string CashierName,
    decimal VatRate);

public sealed record CartLineRequest(
    Guid ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountAmount);

public sealed record PaymentRequest(string Method, decimal Amount, string? ReferenceNumber);

public sealed record CreateSaleRequest(
    IReadOnlyList<CartLineRequest> Lines,
    IReadOnlyList<PaymentRequest> Payments,
    decimal ManualDiscountAmount,
    string? CustomerName,
    string OrderType,
    string? Notes);

public sealed record CreateSaleResponse(
    Guid SaleId,
    string InvoiceNumber,
    decimal GrossTotal,
    decimal DiscountTotal,
    decimal VatAmount,
    decimal NetTotal,
    decimal ChangeDue,
    DateTime SoldAt);
