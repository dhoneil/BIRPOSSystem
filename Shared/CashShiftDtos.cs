namespace BIRPOSSystem.Shared;

public sealed record CashShiftSummaryDto(
    Guid? ShiftId,
    bool IsOpen,
    string BranchName,
    string TerminalCode,
    string CashierName,
    DateTime? OpenedAt,
    DateTime? ClosedAt,
    decimal OpeningCash,
    decimal CashSales,
    decimal CashIn,
    decimal CashOut,
    decimal Drops,
    decimal Payouts,
    decimal ExpectedCash,
    decimal? ClosingCash,
    decimal? Variance,
    string Status);

public sealed record CashMovementDto(
    Guid Id,
    Guid CashShiftId,
    DateTime PostedAt,
    string TerminalCode,
    string Type,
    string TypeLabel,
    string UserName,
    decimal Amount,
    decimal SignedAmount,
    string Reason,
    string ReferenceNumber);

public sealed record CashShiftWorkspaceDto(
    CashShiftSummaryDto Summary,
    IReadOnlyList<CashMovementDto> Movements);

public sealed record OpenCashShiftRequest(string CashierName, decimal OpeningCash);

public sealed record AddCashMovementRequest(string Type, decimal Amount, string Reason);

public sealed record CloseCashShiftRequest(decimal ClosingCash, string? Notes);

public sealed record CashShiftActionResponse(string Message, CashShiftSummaryDto Summary);
