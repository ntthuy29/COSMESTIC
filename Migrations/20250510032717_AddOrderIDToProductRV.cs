using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIDToProductRV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "orderID",
                table: "ProductReView",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "endDate",
                table: "Order",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReView_orderID",
                table: "ProductReView",
                column: "orderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReView_Order_orderID",
                table: "ProductReView",
                column: "orderID",
                principalTable: "Order",
                principalColumn: "orderID",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReView_Order_orderID",
                table: "ProductReView");

            migrationBuilder.DropIndex(
                name: "IX_ProductReView_orderID",
                table: "ProductReView");

            migrationBuilder.DropColumn(
                name: "orderID",
                table: "ProductReView");

            migrationBuilder.AlterColumn<DateTime>(
                name: "endDate",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
