using BIRPOSSystem.Models;
using BIRPOSSystem.Models.Catalog;
using BIRPOSSystem.Models.Sales;
using BIRPOSSystem.Models.Tenancy;
using BIRPOSSystem.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);

        if (await db.Tenants.AnyAsync())
        {
            return;
        }

        var tenant = new Tenant
        {
            Name = "Demo Retail and Cafe Co.",
            TradeName = "Northstar Market Cafe",
            Tin = "000-000-000-000",
            PrimaryBusinessType = BusinessType.Cafe,
            SubscriptionStatus = SubscriptionStatus.Active,
            SubscriptionValidUntil = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
            OfflineGraceDays = 14
        };

        var branch = new Branch
        {
            Tenant = tenant,
            Name = "Main Branch",
            Code = "MAIN",
            Address = "123 Sample Street, Quezon City",
            BirPermitNumber = "BIR-PERMIT-DEMO",
            BirAccreditationNumber = "BIR-ACCRED-DEMO"
        };

        var terminal = new PosTerminal
        {
            Branch = branch,
            Code = "POS-01",
            MachineIdentificationNumber = "MIN-DEMO-001",
            SerialNumber = "SN-DEMO-001",
            PermitNumber = "PTU-DEMO-001"
        };

        var series = new ReceiptSeries
        {
            Terminal = terminal,
            Prefix = "SI",
            CurrentNumber = 100000,
            EndingNumber = 999999
        };

        var shift = new CashShift
        {
            Branch = branch,
            Terminal = terminal,
            CashierName = "Demo Cashier",
            OpeningCash = 2500,
            Status = ShiftStatus.Open
        };

        var categories = new[]
        {
            new ProductCategory { Name = "Coffee", DisplayColor = "#0F766E", SortOrder = 1 },
            new ProductCategory { Name = "Pastries", DisplayColor = "#B45309", SortOrder = 2 },
            new ProductCategory { Name = "Retail Goods", DisplayColor = "#1D4ED8", SortOrder = 3 },
            new ProductCategory { Name = "Essentials", DisplayColor = "#7C3AED", SortOrder = 4 }
        };

        var products = new[]
        {
            Product("COF-AMERICANO", "Americano", categories[0], 95, 80, 10, true),
            Product("COF-LATTE", "Cafe Latte", categories[0], 135, 64, 12, true),
            Product("COF-COLD", "Cold Brew", categories[0], 150, 22, 8, true),
            Product("PAS-CROISSANT", "Butter Croissant", categories[1], 85, 18, 8, true),
            Product("PAS-BROWNIE", "Chocolate Brownie", categories[1], 70, 14, 10, true),
            Product("RTL-RICE5KG", "Premium Rice 5kg", categories[2], 335, 26, 8, false),
            Product("RTL-SUGAR1KG", "Washed Sugar 1kg", categories[2], 82, 6, 10, false),
            Product("ESS-WATER", "Bottled Water 500ml", categories[3], 25, 120, 24, false)
        };

        db.AddRange(tenant, branch, terminal, series, shift);
        db.ProductCategories.AddRange(categories);
        db.Products.AddRange(products);

        await db.SaveChangesAsync();
    }

    private static Product Product(
        string sku,
        string name,
        ProductCategory category,
        decimal price,
        decimal quantity,
        decimal reorderPoint,
        bool isCafeItem) =>
        new()
        {
            Sku = sku,
            Barcode = sku,
            Name = name,
            Category = category,
            Price = price,
            Cost = Math.Round(price * 0.55m, 2),
            QuantityOnHand = quantity,
            ReorderPoint = reorderPoint,
            TrackInventory = true,
            IsCafeItem = isCafeItem,
            IsActive = true
        };

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await SeedUserAsync(
            userManager,
            email: "developer@birpos.local",
            password: "Developer#12345",
            displayName: "System Developer",
            role: AppRoles.Developer);

        await SeedUserAsync(
            userManager,
            email: "owner@birpos.local",
            password: "Owner#12345",
            displayName: "Business Owner",
            role: AppRoles.SuperAdmin);

        await SeedUserAsync(
            userManager,
            email: "admin@birpos.local",
            password: "Admin#12345",
            displayName: "System Admin",
            role: AppRoles.Admin);
    }

    private static async Task SeedUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string displayName,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = displayName,
                RoleLabel = role
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return;
            }
        }
        else
        {
            user.DisplayName = displayName;
            user.RoleLabel = role;
            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
