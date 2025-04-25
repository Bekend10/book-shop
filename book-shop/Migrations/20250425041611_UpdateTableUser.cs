using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Users",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Accounts",
                newName: "email");

            migrationBuilder.AddColumn<int>(
                name: "address_id",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "dob",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "profile_image",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "is_verify",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address_id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "dob",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "profile_image",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "is_verify",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Accounts",
                newName: "username");
        }
    }
}
