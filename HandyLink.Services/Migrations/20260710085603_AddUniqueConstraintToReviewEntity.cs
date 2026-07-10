using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandyLink.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToReviewEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_JobId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_JobId",
                table: "Reviews",
                column: "JobId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_JobId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_JobId",
                table: "Reviews",
                column: "JobId",
                unique: true);
        }
    }
}
