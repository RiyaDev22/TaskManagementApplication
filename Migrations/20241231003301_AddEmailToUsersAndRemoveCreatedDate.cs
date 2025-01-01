using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskListApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToUsersAndRemoveCreatedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Email column
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            // Drop CreatedDate column
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Email column
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            // Add CreatedDate column back
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow);
        }
    }

}
