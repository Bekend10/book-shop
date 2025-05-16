using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumDetailIdOfTableBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "detail_id",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "is_bn",
                table: "BookDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "publisher",
                table: "BookDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "publisher_year",
                table: "BookDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "BookDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_bn",
                table: "BookDetails");

            migrationBuilder.DropColumn(
                name: "publisher",
                table: "BookDetails");

            migrationBuilder.DropColumn(
                name: "publisher_year",
                table: "BookDetails");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "BookDetails");

            migrationBuilder.AddColumn<int>(
                name: "detail_id",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
