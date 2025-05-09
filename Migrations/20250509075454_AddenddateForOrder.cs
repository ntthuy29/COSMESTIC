using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class AddenddateForOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogRevenue");

            migrationBuilder.DropTable(
                name: "DiscountProduct");

            migrationBuilder.AddColumn<DateTime>(
                name: "endDate",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endDate",
                table: "Order");

            migrationBuilder.CreateTable(
                name: "CatalogRevenue",
                columns: table => new
                {
                    catalogRevenueID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "500, 1"),
                    catalogID = table.Column<int>(type: "int", nullable: false),
                    revenueID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogRevenue", x => x.catalogRevenueID);
                    table.ForeignKey(
                        name: "FK_CatalogRevenue_Catalog_catalogID",
                        column: x => x.catalogID,
                        principalTable: "Catalog",
                        principalColumn: "catalogID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogRevenue_Revenue_revenueID",
                        column: x => x.revenueID,
                        principalTable: "Revenue",
                        principalColumn: "revenueID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountProduct",
                columns: table => new
                {
                    discountProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "9000, 1"),
                    discountID = table.Column<int>(type: "int", nullable: false),
                    productID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountProduct", x => x.discountProductID);
                    table.ForeignKey(
                        name: "FK_DiscountProduct_Discount_discountID",
                        column: x => x.discountID,
                        principalTable: "Discount",
                        principalColumn: "discountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountProduct_Product_productID",
                        column: x => x.productID,
                        principalTable: "Product",
                        principalColumn: "productID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRevenue_catalogID",
                table: "CatalogRevenue",
                column: "catalogID");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRevenue_revenueID",
                table: "CatalogRevenue",
                column: "revenueID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountProduct_discountID",
                table: "DiscountProduct",
                column: "discountID");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountProduct_productID",
                table: "DiscountProduct",
                column: "productID");
        }
    }
}
