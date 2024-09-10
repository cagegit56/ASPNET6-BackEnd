using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authorization_and_Authentication.Migrations
{
    /// <inheritdoc />
    public partial class StockDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryProducts");

            migrationBuilder.CreateTable(
                name: "StockProducts",
                columns: table => new
                {
                    ProdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProdPrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgData = table.Column<byte[]>(type: "varbinary(MAX)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockProducts", x => x.ProdId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockProducts");

            migrationBuilder.CreateTable(
                name: "InventoryProducts",
                columns: table => new
                {
                    ProdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgData = table.Column<byte[]>(type: "varbinary(MAX)", nullable: true),
                    ProdName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProdPrice = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryProducts", x => x.ProdId);
                });
        }
    }
}
