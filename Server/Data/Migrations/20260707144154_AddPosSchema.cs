using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BIRPOSSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPosSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoleLabel",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    EntityName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    DisplayColor = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyncOutboxItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    QueuedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UploadedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemType = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    ItemId = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: false),
                    LastError = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncOutboxItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    TradeName = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Tin = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    PrimaryBusinessType = table.Column<int>(type: "INTEGER", nullable: false),
                    SubscriptionStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    SubscriptionValidUntil = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    OfflineGraceDays = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Sku = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Cost = table.Column<decimal>(type: "TEXT", nullable: false),
                    QuantityOnHand = table.Column<decimal>(type: "TEXT", nullable: false),
                    ReorderPoint = table.Column<decimal>(type: "TEXT", nullable: false),
                    TrackInventory = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVatExempt = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCafeItem = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    BirPermitNumber = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    BirAccreditationNumber = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLedgerEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ReferenceType = table.Column<string>(type: "TEXT", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "TEXT", nullable: false),
                    QuantityIn = table.Column<decimal>(type: "TEXT", nullable: false),
                    QuantityOut = table.Column<decimal>(type: "TEXT", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLedgerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryLedgerEntries_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PosTerminals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BranchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    MachineIdentificationNumber = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    PermitNumber = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosTerminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosTerminals_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashShifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BranchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TerminalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CashierName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    OpenedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ClosedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    OpeningCash = table.Column<decimal>(type: "TEXT", nullable: false),
                    ClosingCash = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashShifts_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashShifts_PosTerminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "PosTerminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TerminalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Prefix = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    CurrentNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    EndingNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptSeries_PosTerminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "PosTerminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BranchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TerminalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BusinessDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    BeginningInvoiceNumber = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    EndingInvoiceNumber = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    TransactionCount = table.Column<int>(type: "INTEGER", nullable: false),
                    GrossSales = table.Column<decimal>(type: "TEXT", nullable: false),
                    DiscountTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    VatSales = table.Column<decimal>(type: "TEXT", nullable: false),
                    VatAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    NetSales = table.Column<decimal>(type: "TEXT", nullable: false),
                    VoidTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    RefundTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    ClosedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZReadings_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ZReadings_PosTerminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "PosTerminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BranchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TerminalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ShiftId = table.Column<Guid>(type: "TEXT", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    SoldAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CashierName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    OrderType = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    GrossTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    DiscountTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    VatSales = table.Column<decimal>(type: "TEXT", nullable: false),
                    VatAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    VatExemptSales = table.Column<decimal>(type: "TEXT", nullable: false),
                    NetTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChangeDue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesTransactions_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesTransactions_CashShifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "CashShifts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesTransactions_PosTerminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "PosTerminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesTransactions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SalesTransactionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Sku = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ProductName = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    VatAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    NetAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsVatExempt = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesLines_SalesTransactions_SalesTransactionId",
                        column: x => x.SalesTransactionId,
                        principalTable: "SalesTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SalesTransactionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Method = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesPayments_SalesTransactions_SalesTransactionId",
                        column: x => x.SalesTransactionId,
                        principalTable: "SalesTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OccurredAt",
                table: "AuditLogs",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_TenantId_Code",
                table: "Branches",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashShifts_BranchId",
                table: "CashShifts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CashShifts_TerminalId_Status",
                table: "CashShifts",
                columns: new[] { "TerminalId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLedgerEntries_ProductId",
                table: "InventoryLedgerEntries",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_BranchId_Code",
                table: "PosTerminals",
                columns: new[] { "BranchId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptSeries_TerminalId_Prefix",
                table: "ReceiptSeries",
                columns: new[] { "TerminalId", "Prefix" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesLines_ProductId",
                table: "SalesLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesLines_SalesTransactionId",
                table: "SalesLines",
                column: "SalesTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPayments_SalesTransactionId",
                table: "SalesPayments",
                column: "SalesTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_BranchId",
                table: "SalesTransactions",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_ShiftId",
                table: "SalesTransactions",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_SoldAt",
                table: "SalesTransactions",
                column: "SoldAt");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_TenantId",
                table: "SalesTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_TerminalId_InvoiceNumber",
                table: "SalesTransactions",
                columns: new[] { "TerminalId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SyncOutboxItems_QueuedAt",
                table: "SyncOutboxItems",
                column: "QueuedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SyncOutboxItems_Status",
                table: "SyncOutboxItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Tin",
                table: "Tenants",
                column: "Tin");

            migrationBuilder.CreateIndex(
                name: "IX_ZReadings_BranchId",
                table: "ZReadings",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ZReadings_TerminalId_BusinessDate",
                table: "ZReadings",
                columns: new[] { "TerminalId", "BusinessDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "InventoryLedgerEntries");

            migrationBuilder.DropTable(
                name: "ReceiptSeries");

            migrationBuilder.DropTable(
                name: "SalesLines");

            migrationBuilder.DropTable(
                name: "SalesPayments");

            migrationBuilder.DropTable(
                name: "SyncOutboxItems");

            migrationBuilder.DropTable(
                name: "ZReadings");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "SalesTransactions");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "CashShifts");

            migrationBuilder.DropTable(
                name: "PosTerminals");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RoleLabel",
                table: "AspNetUsers");
        }
    }
}
