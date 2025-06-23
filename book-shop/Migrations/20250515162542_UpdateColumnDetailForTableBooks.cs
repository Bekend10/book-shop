using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnDetailForTableBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "detail_id",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "detail_id",
                table: "Books");
        }
    }
}
