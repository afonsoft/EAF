using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eaf.ProjectName.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionPaymentMassNotificationUserDelegationAndSubscribableEdition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionEndDateUtc",
                table: "AbpTenants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BiannualPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultPaymentPeriodType",
                table: "AbpEditions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AbpEditions",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "SubscribableEdition");

            migrationBuilder.AddColumn<int>(
                name: "ExpiringEditionId",
                table: "AbpEditions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PermanentPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuarterlyPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrialDayCount",
                table: "AbpEditions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WaitingDayAfterExpire",
                table: "AbpEditions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WeeklyPrice",
                table: "AbpEditions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EafMassNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Severity = table.Column<byte>(type: "tinyint", nullable: false),
                    TargetUserIds = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TargetRoleIds = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TargetOrganizationUnitIds = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SendToAllUsers = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EafMassNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EafSubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    EditionId = table.Column<int>(type: "int", nullable: false),
                    EditionPaymentType = table.Column<int>(type: "int", nullable: false),
                    PaymentPeriodType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Gateway = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExternalPaymentId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    GatewayResponse = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PaymentTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EafSubscriptionPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EafUserDelegations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    SourceUserId = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserId = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
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
                    table.PrimaryKey("PK_EafUserDelegations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EafMassNotifications");

            migrationBuilder.DropTable(
                name: "EafSubscriptionPayments");

            migrationBuilder.DropTable(
                name: "EafUserDelegations");

            migrationBuilder.DropColumn(
                name: "SubscriptionEndDateUtc",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "AnnualPrice",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "BiannualPrice",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "DailyPrice",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "DefaultPaymentPeriodType",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "ExpiringEditionId",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "MonthlyPrice",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "PermanentPrice",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "QuarterlyPrice",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "TrialDayCount",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "WaitingDayAfterExpire",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "WeeklyPrice",
                table: "AbpEditions");
        }
    }
}
