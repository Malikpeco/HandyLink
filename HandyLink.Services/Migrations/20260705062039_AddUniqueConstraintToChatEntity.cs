using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandyLink.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToChatEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chats_JobId",
                table: "Chats");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_JobId",
                table: "Chats",
                column: "JobId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chats_JobId",
                table: "Chats");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_JobId",
                table: "Chats",
                column: "JobId",
                unique: true);
        }
    }
}
