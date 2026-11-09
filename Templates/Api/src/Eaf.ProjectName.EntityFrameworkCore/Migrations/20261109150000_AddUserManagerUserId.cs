using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eaf.ProjectName.Migrations
{
    /// <inheritdoc />
    public partial class AddUserManagerUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ManagerUserId",
                table: "AbpUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_ManagerUserId",
                table: "AbpUsers",
                column: "ManagerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_ManagerUserId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "ManagerUserId",
                table: "AbpUsers");
        }
    }
}
