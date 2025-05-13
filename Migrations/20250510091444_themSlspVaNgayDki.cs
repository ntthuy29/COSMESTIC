using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COSMESTIC.Migrations
{
    /// <inheritdoc />
    public partial class themSlspVaNgayDki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "createdDate",
                table: "User",
                type: "datetime2",
                nullable: true);

           

            migrationBuilder.AddColumn<int>(
                name: "totalItems",
                table: "Order",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "totalItems",
                table: "Order");

           
        }
    }
}
