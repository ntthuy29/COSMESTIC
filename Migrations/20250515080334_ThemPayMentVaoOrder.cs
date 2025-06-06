using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class ThemPayMentVaoOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "payMethod",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payMethod",
                table: "Order");
        }
    }
}
