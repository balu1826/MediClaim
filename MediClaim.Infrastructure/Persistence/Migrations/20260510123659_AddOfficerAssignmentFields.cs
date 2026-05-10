using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediClaim.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOfficerAssignmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Provider_ProviderId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Provider_Tenants_TenantId",
                table: "Provider");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provider",
                table: "Provider");

            migrationBuilder.RenameTable(
                name: "Provider",
                newName: "Providers");

            migrationBuilder.RenameIndex(
                name: "IX_Provider_TenantId",
                table: "Providers",
                newName: "IX_Providers_TenantId");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAssignedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedOfficerId",
                table: "Claims",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PendingAssignment",
                table: "Claims",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Providers",
                table: "Providers",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_AssignedOfficerId",
                table: "Claims",
                column: "AssignedOfficerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Providers_ProviderId",
                table: "Claims",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "ProviderId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Users_AssignedOfficerId",
                table: "Claims",
                column: "AssignedOfficerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Providers_Tenants_TenantId",
                table: "Providers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Providers_ProviderId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Users_AssignedOfficerId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Providers_Tenants_TenantId",
                table: "Providers");

            migrationBuilder.DropIndex(
                name: "IX_Claims_AssignedOfficerId",
                table: "Claims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Providers",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "LastAssignedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssignedOfficerId",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "PendingAssignment",
                table: "Claims");

            migrationBuilder.RenameTable(
                name: "Providers",
                newName: "Provider");

            migrationBuilder.RenameIndex(
                name: "IX_Providers_TenantId",
                table: "Provider",
                newName: "IX_Provider_TenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provider",
                table: "Provider",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Provider_ProviderId",
                table: "Claims",
                column: "ProviderId",
                principalTable: "Provider",
                principalColumn: "ProviderId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Provider_Tenants_TenantId",
                table: "Provider",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
