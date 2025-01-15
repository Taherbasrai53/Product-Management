using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Product_Management.Migrations
{
    /// <inheritdoc />
    public partial class storesadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RetailerID",
                table: "Products",
                newName: "StoreId");

            migrationBuilder.AddColumn<int>(
                name: "Performance",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RetailerID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Franchisee = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropColumn(
                name: "Performance",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "Products",
                newName: "RetailerID");
        }
    }
}
