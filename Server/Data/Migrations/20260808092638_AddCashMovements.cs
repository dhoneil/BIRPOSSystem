using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BIRPOSSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCashMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CashShiftId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BranchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TerminalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 240, nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashMovements_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashMovements_CashShifts_CashShiftId",
                        column: x => x.CashShiftId,
                        principalTable: "CashShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashMovements_PosTerminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "PosTerminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashMovements_BranchId",
                table: "CashMovements",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CashMovements_CashShiftId_PostedAt",
                table: "CashMovements",
                columns: new[] { "CashShiftId", "PostedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CashMovements_TerminalId_PostedAt",
                table: "CashMovements",
                columns: new[] { "TerminalId", "PostedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashMovements");
        }
    }
}
