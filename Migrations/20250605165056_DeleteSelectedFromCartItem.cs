using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSelectedFromCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Selected",
                table: "CartItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Selected",
                table: "CartItem",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
