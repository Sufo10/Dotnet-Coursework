using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coursework.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class bookcar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "CustomerFileUpload",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CustomerFileUpload",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Customer",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "CustomerBooking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    customerId = table.Column<string>(type: "text", nullable: false),
                    CarId = table.Column<string>(type: "text", nullable: false),
                    ApprovedBy = table.Column<string>(type: "text", nullable: false),
                    RentStartdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RentEnddate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBooking", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFileUpload_UserId",
                table: "CustomerFileUpload",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFileUpload_AspNetUsers_UserId",
                table: "CustomerFileUpload",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFileUpload_AspNetUsers_UserId",
                table: "CustomerFileUpload");

            migrationBuilder.DropTable(
                name: "CustomerBooking");

            migrationBuilder.DropIndex(
                name: "IX_CustomerFileUpload_UserId",
                table: "CustomerFileUpload");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CustomerFileUpload",
                newName: "UserID");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "CustomerFileUpload",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Customer",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
