namespace BIRPOSSystem.Shared;

public sealed record ZReadingDto(
    string BranchName,
    string TerminalCode,
    DateOnly BusinessDate,
    string BeginningInvoiceNumber,
    string EndingInvoiceNumber,
    int TransactionCount,
    decimal GrossSales,
    decimal DiscountTotal,
    decimal VatSales,
    decimal VatAmount,
    decimal NetSales,
    decimal VoidTotal,
    decimal RefundTotal,
    DateTime? ClosedAt);

public sealed record SyncStatusDto(
    bool IsOnline,
    int PendingUploads,
    DateTime LastSuccessfulSyncAt,
    string LastMessage,
    string SubscriptionStatus,
    DateOnly SubscriptionValidUntil);

public sealed record SyncUploadResponse(
    int UploadedItems,
    DateTime SyncedAt,
    string Message);
