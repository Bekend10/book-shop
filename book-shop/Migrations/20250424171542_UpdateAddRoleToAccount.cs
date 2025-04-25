using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book_shop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAddRoleToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Accounts_role_id",
                table: "Accounts",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Roles_role_id",
                table: "Accounts",
                column: "role_id",
                principalTable: "Roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Roles_role_id",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_role_id",
                table: "Accounts");
        }
    }
}
