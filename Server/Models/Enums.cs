namespace BIRPOSSystem.Models;

public enum BusinessType
{
    Retail = 1,
    Cafe = 2,
    Restaurant = 3,
    Grocery = 4,
    Pharmacy = 5,
    Services = 6
}

public enum SubscriptionStatus
{
    Trial = 1,
    Active = 2,
    GracePeriod = 3,
    Expired = 4,
    Suspended = 5
}

public enum SaleStatus
{
    Completed = 1,
    Voided = 2,
    Refunded = 3
}

public enum ShiftStatus
{
    Open = 1,
    Closed = 2
}

public enum CashMovementType
{
    OpeningFloat = 1,
    CashIn = 2,
    CashOut = 3,
    Drop = 4,
    Payout = 5,
    ClosingCount = 6
}

public enum SyncItemStatus
{
    Pending = 1,
    Uploaded = 2,
    Failed = 3
}
