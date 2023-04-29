using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coursework.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "payment",
                table: "CustomerBooking",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "rented",
                table: "CustomerBooking",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment",
                table: "CustomerBooking");

            migrationBuilder.DropColumn(
                name: "rented",
                table: "CustomerBooking");
        }
    }
}
