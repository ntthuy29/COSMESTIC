using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryID",
                table: "Order",
                type: "int",
                nullable: true);

            

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeliveryID",
                table: "Order",
                column: "DeliveryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryIFMT_DeliveryID",
                table: "Order",
                column: "DeliveryID",
                principalTable: "DeliveryIFMT",
                principalColumn: "deliveryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryIFMT_DeliveryID",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_DeliveryID",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveryID",
                table: "Order");

            
        }
    }
}
