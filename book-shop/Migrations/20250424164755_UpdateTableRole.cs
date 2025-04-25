using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_user_id",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_user_id",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "password",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "role_id",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.role_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_user_id",
                table: "Accounts",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_user_id",
                table: "Accounts",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_user_id",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_user_id",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "password",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "role_id",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
    }
}
