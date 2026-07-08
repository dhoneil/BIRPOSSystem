using BIRPOSSystem.Models.Catalog;
using BIRPOSSystem.Models.Compliance;
using BIRPOSSystem.Models.Inventory;
using BIRPOSSystem.Models.Sales;
using BIRPOSSystem.Models.Sync;
using BIRPOSSystem.Models.Tenancy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BIRPOSSystem.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<PosTerminal> PosTerminals => Set<PosTerminal>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryLedgerEntry> InventoryLedgerEntries => Set<InventoryLedgerEntry>();
    public DbSet<ReceiptSeries> ReceiptSeries => Set<ReceiptSeries>();
    public DbSet<CashShift> CashShifts => Set<CashShift>();
    public DbSet<SalesTransaction> SalesTransactions => Set<SalesTransaction>();
    public DbSet<SalesLine> SalesLines => Set<SalesLine>();
    public DbSet<SalesPayment> SalesPayments => Set<SalesPayment>();
    public DbSet<ZReading> ZReadings => Set<ZReading>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SyncOutboxItem> SyncOutboxItems => Set<SyncOutboxItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tenant>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(160);
            entity.Property(x => x.TradeName).HasMaxLength(160);
            entity.Property(x => x.Tin).HasMaxLength(32);
            entity.HasIndex(x => x.Tin);
        });

        builder.Entity<Branch>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(160);
            entity.Property(x => x.Code).HasMaxLength(32);
            entity.Property(x => x.Address).HasMaxLength(300);
            entity.Property(x => x.BirPermitNumber).HasMaxLength(80);
            entity.Property(x => x.BirAccreditationNumber).HasMaxLength(80);
            entity.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        });

        builder.Entity<PosTerminal>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(32);
            entity.Property(x => x.MachineIdentificationNumber).HasMaxLength(80);
            entity.Property(x => x.SerialNumber).HasMaxLength(80);
            entity.Property(x => x.PermitNumber).HasMaxLength(80);
            entity.HasIndex(x => new { x.BranchId, x.Code }).IsUnique();
        });

        builder.Entity<ProductCategory>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(120);
            entity.Property(x => x.DisplayColor).HasMaxLength(16);
        });

        builder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Sku).HasMaxLength(64);
            entity.Property(x => x.Barcode).HasMaxLength(64);
            entity.Property(x => x.Name).HasMaxLength(180);
            entity.HasIndex(x => x.Sku).IsUnique();
            entity.HasIndex(x => x.Barcode);
        });

        builder.Entity<ReceiptSeries>(entity =>
        {
            entity.Property(x => x.Prefix).HasMaxLength(16);
            entity.HasIndex(x => new { x.TerminalId, x.Prefix }).IsUnique();
        });

        builder.Entity<CashShift>(entity =>
        {
            entity.Property(x => x.CashierName).HasMaxLength(120);
            entity.HasIndex(x => new { x.TerminalId, x.Status });
        });

        builder.Entity<SalesTransaction>(entity =>
        {
            entity.Property(x => x.InvoiceNumber).HasMaxLength(32);
            entity.Property(x => x.CashierName).HasMaxLength(120);
            entity.Property(x => x.CustomerName).HasMaxLength(160);
            entity.Property(x => x.OrderType).HasMaxLength(32);
            entity.Property(x => x.Notes).HasMaxLength(500);
            entity.HasIndex(x => new { x.TerminalId, x.InvoiceNumber }).IsUnique();
            entity.HasIndex(x => x.SoldAt);
        });

        builder.Entity<SalesLine>(entity =>
        {
            entity.Property(x => x.Sku).HasMaxLength(64);
            entity.Property(x => x.ProductName).HasMaxLength(180);
        });

        builder.Entity<SalesPayment>(entity =>
        {
            entity.Property(x => x.Method).HasMaxLength(32);
            entity.Property(x => x.ReferenceNumber).HasMaxLength(120);
        });

        builder.Entity<ZReading>(entity =>
        {
            entity.Property(x => x.BeginningInvoiceNumber).HasMaxLength(32);
            entity.Property(x => x.EndingInvoiceNumber).HasMaxLength(32);
            entity.HasIndex(x => new { x.TerminalId, x.BusinessDate }).IsUnique();
        });

        builder.Entity<AuditLog>(entity =>
        {
            entity.Property(x => x.UserName).HasMaxLength(120);
            entity.Property(x => x.Action).HasMaxLength(80);
            entity.Property(x => x.EntityName).HasMaxLength(120);
            entity.Property(x => x.EntityId).HasMaxLength(80);
            entity.Property(x => x.Details).HasMaxLength(1000);
            entity.HasIndex(x => x.OccurredAt);
        });

        builder.Entity<SyncOutboxItem>(entity =>
        {
            entity.Property(x => x.ItemType).HasMaxLength(80);
            entity.Property(x => x.ItemId).HasMaxLength(80);
            entity.Property(x => x.LastError).HasMaxLength(500);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.QueuedAt);
        });
    }
}
