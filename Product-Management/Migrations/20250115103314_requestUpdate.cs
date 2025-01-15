using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Product_Management.Migrations
{
    /// <inheritdoc />
    public partial class requestUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RetailerID",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetailerID",
                table: "Requests");
        }
    }
}
