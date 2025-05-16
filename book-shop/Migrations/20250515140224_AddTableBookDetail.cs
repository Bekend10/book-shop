using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    /// <inheritdoc />
    public partial class AddTableBookDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "author",
                table: "Books",
                newName: "image_url");

            migrationBuilder.AddColumn<int>(
                name: "author_id",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "price",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BookDetails",
                columns: table => new
                {
                    detail_id = table.Column<int>(type: "int", nullable: false),
                    book_id = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    file_demo_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    number_of_page = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookDetails", x => x.detail_id);
                    table.ForeignKey(
                        name: "FK_BookDetails_Books_detail_id",
                        column: x => x.detail_id,
                        principalTable: "Books",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookDetails");

            migrationBuilder.DropColumn(
                name: "author_id",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "price",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "image_url",
                table: "Books",
                newName: "author");
        }
    }
}
