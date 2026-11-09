using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eaf.ProjectName.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicPropertyMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EafDynamicPropertyMetadata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    PropertyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_EafDynamicPropertyMetadata", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EafDynamicPropertyMetadata_TenantId_PropertyName",
                table: "EafDynamicPropertyMetadata",
                columns: new[] { "TenantId", "PropertyName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EafDynamicPropertyMetadata");
        }
    }
}
