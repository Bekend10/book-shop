using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    public partial class UpdateBookDetailRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa FK từ BookDetails nếu tồn tại (nên đảm bảo FK này không tồn tại trước hoặc kiểm tra tay)
            // Xóa PK hiện tại trên detail_id
            migrationBuilder.DropPrimaryKey(
                name: "PK_BookDetails",
                table: "BookDetails");

            // Xóa cột detail_id cũ
            migrationBuilder.DropColumn(
                name: "detail_id",
                table: "BookDetails");

            // Thêm lại cột detail_id với IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "detail_id",
                table: "BookDetails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // Tạo lại Primary Key
            migrationBuilder.AddPrimaryKey(
                name: "PK_BookDetails",
                table: "BookDetails",
                column: "detail_id");

            // Tạo Index và FK từ Books -> BookDetails
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa FK và Index mới
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookDetails_detail_id",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_detail_id",
                table: "Books");

            // Xóa PK mới
            migrationBuilder.DropPrimaryKey(
                name: "PK_BookDetails",
                table: "BookDetails");

            // Xóa cột detail_id (IDENTITY)
            migrationBuilder.DropColumn(
                name: "detail_id",
                table: "BookDetails");

            // Thêm lại cột detail_id không có IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "detail_id",
                table: "BookDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Tạo lại PK
            migrationBuilder.AddPrimaryKey(
                name: "PK_BookDetails",
                table: "BookDetails",
                column: "detail_id");

            // Tạo lại FK cũ nếu có (có thể tùy chỉnh lại nếu cần)
            migrationBuilder.AddForeignKey(
                name: "FK_BookDetails_Books_detail_id",
                table: "BookDetails",
                column: "detail_id",
                principalTable: "Books",
                principalColumn: "book_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}