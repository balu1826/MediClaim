using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediClaim.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingLimitToPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RemainingLimit",
                table: "Policies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingLimit",
                table: "Policies");
        }
    }
}
