using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    public partial class UpdateBookDetailRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa Foreign Key cũ (nếu có)
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_Books_BookDetails_detail_id'
                )
                ALTER TABLE Books DROP CONSTRAINT FK_Books_BookDetails_detail_id;
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_BookDetails_Books_detail_id'
                )
                ALTER TABLE BookDetails DROP CONSTRAINT FK_BookDetails_Books_detail_id;
            ");

            // Xóa Primary Key hiện tại (nếu có)
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.key_constraints 
                    WHERE name = 'PK_BookDetails'
                )
                ALTER TABLE BookDetails DROP CONSTRAINT PK_BookDetails;
            ");

            // Xóa cột detail_id cũ (nếu có)
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE Name = N'detail_id' AND Object_ID = Object_ID(N'BookDetails')
                )
                ALTER TABLE BookDetails DROP COLUMN detail_id;
            ");

            // Thêm lại cột detail_id với IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "detail_id",
                table: "BookDetails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // Tạo lại Primary Key mới
            migrationBuilder.AddPrimaryKey(
                name: "PK_BookDetails",
                table: "BookDetails",
                column: "detail_id");

            // Thêm cột detail_id vào Books nếu chưa có
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE Name = N'detail_id' AND Object_ID = Object_ID(N'Books')
                )
                ALTER TABLE Books ADD detail_id INT NULL;
            ");

            // Tạo Index và FK mới (1-1)
            migrationBuilder.CreateIndex(
                name: "IX_Books_detail_id",
                table: "Books",
                column: "detail_id",
                unique: true,
                filter: "[detail_id] IS NOT NULL");

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
            // Xóa FK và Index
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

            // Thêm lại cột detail_id cũ (không IDENTITY)
            migrationBuilder.AddColumn<int>(
                name: "detail_id",
                table: "BookDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Tạo lại Primary Key cũ
            migrationBuilder.AddPrimaryKey(
                name: "PK_BookDetails",
                table: "BookDetails",
                column: "detail_id");
        }
    }
}