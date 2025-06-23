using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelasionshipBookAndBookDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookDetails_detail_id",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_detail_id",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_BookDetails_book_id",
                table: "BookDetails",
                column: "book_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookDetails_Books_book_id",
                table: "BookDetails",
                column: "book_id",
                principalTable: "Books",
                principalColumn: "book_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookDetails_Books_book_id",
                table: "BookDetails");

            migrationBuilder.DropIndex(
                name: "IX_BookDetails_book_id",
                table: "BookDetails");

            migrationBuilder.CreateIndex(
                name: "IX_Books_detail_id",
                table: "Books",
                column: "detail_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookDetails_detail_id",
                table: "Books",
                column: "detail_id",
                principalTable: "BookDetails",
                principalColumn: "detail_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
