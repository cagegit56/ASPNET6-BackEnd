using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authorization_and_Authentication.Migrations
{
    /// <inheritdoc />
    public partial class ProductsDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.CreateTable(
                name: "ImgProducts",
                columns: table => new
                {
                    prodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    prodName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    prodPrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    imgData = table.Column<byte[]>(type: "varbinary(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImgProducts", x => x.prodId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImgProducts");

            migrationBuilder.CreateTable(
                name: "Products",
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
                    table.PrimaryKey("PK_Products", x => x.prodId);
                });
        }
    }
}
