using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class setNullContrainst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Catalog_catalogID",
                table: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Catalog_catalogID",
                table: "Product",
                column: "catalogID",
                principalTable: "Catalog",
                principalColumn: "catalogID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Catalog_catalogID",
                table: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Catalog_catalogID",
                table: "Product",
                column: "catalogID",
                principalTable: "Catalog",
                principalColumn: "catalogID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
