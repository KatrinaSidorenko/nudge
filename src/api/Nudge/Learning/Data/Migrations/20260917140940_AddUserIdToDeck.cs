using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nudge.Learning.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToDeck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_id",
                schema: "learning",
                table: "deck",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_deck_user_id",
                schema: "learning",
                table: "deck",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_deck_user_id",
                schema: "learning",
                table: "deck");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "learning",
                table: "deck");
        }
    }
}
