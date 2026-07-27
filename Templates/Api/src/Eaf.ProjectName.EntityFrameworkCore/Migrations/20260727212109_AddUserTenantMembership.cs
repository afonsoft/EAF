using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eaf.ProjectName.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTenantMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpUserTenantMemberships",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    TenantUserId = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserTenantMemberships", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserTenantMemberships_TenantUserId",
                table: "AbpUserTenantMemberships",
                column: "TenantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserTenantMemberships_UserId_TenantId",
                table: "AbpUserTenantMemberships",
                columns: new[] { "UserId", "TenantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpUserTenantMemberships");
        }
    }
}
