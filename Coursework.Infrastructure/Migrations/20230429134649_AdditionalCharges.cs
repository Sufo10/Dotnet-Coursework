using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coursework.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalCharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionCharges",
                table: "AdditionCharges");

            migrationBuilder.RenameTable(
                name: "AdditionCharges",
                newName: "AdditionalCharges");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionalCharges",
                table: "AdditionalCharges",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionalCharges",
                table: "AdditionalCharges");

            migrationBuilder.RenameTable(
                name: "AdditionalCharges",
                newName: "AdditionCharges");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionCharges",
                table: "AdditionCharges",
                column: "Id");
        }
    }
}
