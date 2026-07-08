using Microsoft.AspNetCore.Identity;

namespace BIRPOSSystem.Data;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string RoleLabel { get; set; } = "Cashier";
}

