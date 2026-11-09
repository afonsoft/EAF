using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eaf.ProjectName.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPasskeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EafUserPasskeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CredentialId = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    PublicKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    SignCount = table.Column<long>(type: "bigint", nullable: false),
                    Transports = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AaGuid = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    AttestationData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsBackupEligible = table.Column<bool>(type: "bit", nullable: false),
                    LastUsedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EafUserPasskeys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EafUserPasskeys_CredentialId",
                table: "EafUserPasskeys",
                column: "CredentialId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EafUserPasskeys_TenantId_UserId",
                table: "EafUserPasskeys",
                columns: new[] { "TenantId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EafUserPasskeys");
        }
    }
}
