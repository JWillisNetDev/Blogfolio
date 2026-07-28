using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blogfolio.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuditing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "TodoItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedByUserId",
                table: "TodoItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_CreatedByUserId",
                table: "TodoItems",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_LastUpdatedByUserId",
                table: "TodoItems",
                column: "LastUpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_AspNetUsers_CreatedByUserId",
                table: "TodoItems",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_AspNetUsers_LastUpdatedByUserId",
                table: "TodoItems",
                column: "LastUpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AspNetUsers_CreatedByUserId",
                table: "TodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AspNetUsers_LastUpdatedByUserId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_CreatedByUserId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_LastUpdatedByUserId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "TodoItems");
        }
    }
}
