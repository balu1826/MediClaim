using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediClaim.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnToPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Policies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Policies");
        }
    }
}
