using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_user_id1",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_user_id1",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "Accounts");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_user_id",
                table: "Accounts",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_user_id",
                table: "Accounts",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_user_id",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_user_id",
                table: "Accounts");

            migrationBuilder.AddColumn<int>(
                name: "user_id1",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_user_id1",
                table: "Accounts",
                column: "user_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_user_id1",
                table: "Accounts",
                column: "user_id1",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
