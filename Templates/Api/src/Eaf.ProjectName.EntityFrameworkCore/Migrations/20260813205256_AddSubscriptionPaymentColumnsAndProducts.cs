using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eaf.ProjectName.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionPaymentColumnsAndProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorUrl",
                table: "EafSubscriptionPayments",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "EafSubscriptionPayments",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewaySubscriptionId",
                table: "EafSubscriptionPayments",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "EafSubscriptionPayments",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProrationPayment",
                table: "EafSubscriptionPayments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "EafSubscriptionPayments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SuccessUrl",
                table: "EafSubscriptionPayments",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EafSubscriptionPaymentProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    SubscriptionPaymentId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EafSubscriptionPaymentProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EafSubscriptionPaymentProducts_EafSubscriptionPayments_SubscriptionPaymentId",
                        column: x => x.SubscriptionPaymentId,
                        principalTable: "EafSubscriptionPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EafSubscriptionPaymentProducts_SubscriptionPaymentId",
                table: "EafSubscriptionPaymentProducts",
                column: "SubscriptionPaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EafSubscriptionPaymentProducts");

            migrationBuilder.DropColumn(
                name: "ErrorUrl",
                table: "EafSubscriptionPayments");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "EafSubscriptionPayments");

            migrationBuilder.DropColumn(
                name: "GatewaySubscriptionId",
                table: "EafSubscriptionPayments");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "EafSubscriptionPayments");

            migrationBuilder.DropColumn(
                name: "IsProrationPayment",
                table: "EafSubscriptionPayments");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "EafSubscriptionPayments");

            migrationBuilder.DropColumn(
                name: "SuccessUrl",
                table: "EafSubscriptionPayments");
        }
    }
}
