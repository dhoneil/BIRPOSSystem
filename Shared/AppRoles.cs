namespace BIRPOSSystem.Shared;

public static class AppRoles
{
    public const string Developer = "Developer";
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Cashier = "Cashier";
    public const string InventoryStaff = "Inventory Staff";
    public const string Auditor = "Auditor";

    public static readonly string[] All =
    [
        Developer,
        SuperAdmin,
        Admin,
        Manager,
        Cashier,
        InventoryStaff,
        Auditor
    ];
}
