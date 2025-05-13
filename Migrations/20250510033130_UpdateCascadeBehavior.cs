using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReView_Order_orderID",
                table: "ProductReView");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReView_Order_orderID",
                table: "ProductReView",
                column: "orderID",
                principalTable: "Order",
                principalColumn: "orderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReView_Order_orderID",
                table: "ProductReView");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReView_Order_orderID",
                table: "ProductReView",
                column: "orderID",
                principalTable: "Order",
                principalColumn: "orderID",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
