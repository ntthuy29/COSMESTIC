using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedToCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Selected",
                table: "CartItem",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Selected",
                table: "CartItem");
        }
    }
}
