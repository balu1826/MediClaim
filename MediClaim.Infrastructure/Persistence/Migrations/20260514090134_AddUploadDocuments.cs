using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediClaim.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ProviderId",
                table: "Claims",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ClaimDocuments",
                columns: table => new
                {
                    ClaimDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimDocuments", x => x.ClaimDocumentId);
                    table.ForeignKey(
                        name: "FK_ClaimDocuments_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "ClaimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_ClaimId",
                table: "ClaimDocuments",
                column: "ClaimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimDocuments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProviderId",
                table: "Claims",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
