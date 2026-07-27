using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eaf.ProjectName.Migrations
{
    /// <inheritdoc />
    public partial class AddContextualChatFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenantAddress_AbpUsers_CreatorUserId",
                table: "AbpTenantAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenantAddress_AbpUsers_DeleterUserId",
                table: "AbpTenantAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenantAddress_AbpUsers_LastModifierUserId",
                table: "AbpTenantAddress");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenantAddress_CreatorUserId",
                table: "AbpTenantAddress");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenantAddress_DeleterUserId",
                table: "AbpTenantAddress");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenantAddress_LastModifierUserId",
                table: "AbpTenantAddress");

            migrationBuilder.AddColumn<string>(
                name: "ClientMessageId",
                table: "EafChatMessages",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContextType",
                table: "EafChatMessages",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId",
                table: "EafChatMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GameId",
                table: "EafChatMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MatchId",
                table: "EafChatMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parameters",
                table: "AbpAuditLogs",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientMessageId",
                table: "EafChatMessages");

            migrationBuilder.DropColumn(
                name: "ContextType",
                table: "EafChatMessages");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "EafChatMessages");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "EafChatMessages");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "EafChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Parameters",
                table: "AbpAuditLogs",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenantAddress_CreatorUserId",
                table: "AbpTenantAddress",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenantAddress_DeleterUserId",
                table: "AbpTenantAddress",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenantAddress_LastModifierUserId",
                table: "AbpTenantAddress",
                column: "LastModifierUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenantAddress_AbpUsers_CreatorUserId",
                table: "AbpTenantAddress",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenantAddress_AbpUsers_DeleterUserId",
                table: "AbpTenantAddress",
                column: "DeleterUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenantAddress_AbpUsers_LastModifierUserId",
                table: "AbpTenantAddress",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");
        }
    }
}
