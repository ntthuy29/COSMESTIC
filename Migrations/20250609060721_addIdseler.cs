using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class addIdseler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SellerID",
                table: "OrderDetail",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_SellerID",
                table: "OrderDetail",
                column: "SellerID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_User_SellerID",
                table: "OrderDetail",
                column: "SellerID",
                principalTable: "User",
                principalColumn: "userID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_User_SellerID",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_SellerID",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "SellerID",
                table: "OrderDetail");
        }
    }
}
