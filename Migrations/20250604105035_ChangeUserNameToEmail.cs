using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserNameToEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "username",
                table: "Account",
                newName: "email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "email",
                table: "Account",
                newName: "username");
        }
    }
}
