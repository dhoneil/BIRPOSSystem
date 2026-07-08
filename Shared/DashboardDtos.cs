namespace BIRPOSSystem.Shared;

public sealed record DashboardSummaryDto(
    decimal TodaySales,
    int TodayTransactions,
    decimal CashInDrawer,
    int PendingSyncItems,
    string ActiveShift,
    string SubscriptionStatus,
    DateTime LastSyncAt);

public sealed record RecentSaleDto(
    string InvoiceNumber,
    DateTime SoldAt,
    string CashierName,
    decimal NetTotal,
    string Status);

public sealed record DashboardDto(
    DashboardSummaryDto Summary,
    IReadOnlyList<RecentSaleDto> RecentSales,
    IReadOnlyList<InventoryAlertDto> InventoryAlerts);

public sealed record InventoryAlertDto(
    string Sku,
    string ProductName,
    decimal QuantityOnHand,
    decimal ReorderPoint);
