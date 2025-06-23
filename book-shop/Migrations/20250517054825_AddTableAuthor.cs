using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    author_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nationally = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.author_id);
                });

            migrationBuilder.CreateTable(
                name: "BookAuthors",
                columns: table => new
                {
                    authorsauthor_id = table.Column<int>(type: "int", nullable: false),
                    booksbook_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuthors", x => new { x.authorsauthor_id, x.booksbook_id });
                    table.ForeignKey(
                        name: "FK_BookAuthors_Authors_authorsauthor_id",
                        column: x => x.authorsauthor_id,
                        principalTable: "Authors",
                        principalColumn: "author_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookAuthors_Books_booksbook_id",
                        column: x => x.booksbook_id,
                        principalTable: "Books",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthors_booksbook_id",
                table: "BookAuthors",
                column: "booksbook_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuthors");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
