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
                name: "ImgProducts");

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    ProdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProdPrice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImgData = table.Column<byte[]>(type: "varbinary(MAX)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.ProdId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.CreateTable(
                name: "ImgProducts",
                columns: table => new
                {
                    prodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    imgData = table.Column<byte[]>(type: "varbinary(MAX)", nullable: true),
                    prodName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    prodPrice = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImgProducts", x => x.prodId);
                });
        }
    }
}
