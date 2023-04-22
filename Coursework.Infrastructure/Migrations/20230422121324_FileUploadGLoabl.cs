using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coursework.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FileUploadGLoabl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFileUpload_Customer_CustomerId",
                table: "CustomerFileUpload");

            migrationBuilder.DropIndex(
                name: "IX_CustomerFileUpload_CustomerId",
                table: "CustomerFileUpload");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "CustomerFileUpload");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CustomerFileUpload",
                type: "text",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.DropIndex(
                name: "IX_CustomerFileUpload_UserId",
                table: "CustomerFileUpload");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CustomerFileUpload");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "CustomerFileUpload",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFileUpload_CustomerId",
                table: "CustomerFileUpload",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFileUpload_Customer_CustomerId",
                table: "CustomerFileUpload",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
