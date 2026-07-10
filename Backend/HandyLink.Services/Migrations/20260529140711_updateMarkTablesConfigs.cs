using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandyLink.Services.Migrations
{
    /// <inheritdoc />
    public partial class updateMarkTablesConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobCompletionMarks_JobId",
                table: "JobCompletionMarks");

            migrationBuilder.DropIndex(
                name: "IX_JobCancellationMarks_JobId",
                table: "JobCancellationMarks");

            migrationBuilder.CreateIndex(
                name: "IX_JobCompletionMarks_JobId_MarkedByUserId",
                table: "JobCompletionMarks",
                columns: new[] { "JobId", "MarkedByUserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_JobCancellationMarks_JobId_MarkedByUserId",
                table: "JobCancellationMarks",
                columns: new[] { "JobId", "MarkedByUserId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobCompletionMarks_JobId_MarkedByUserId",
                table: "JobCompletionMarks");

            migrationBuilder.DropIndex(
                name: "IX_JobCancellationMarks_JobId_MarkedByUserId",
                table: "JobCancellationMarks");

            migrationBuilder.CreateIndex(
                name: "IX_JobCompletionMarks_JobId",
                table: "JobCompletionMarks",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCancellationMarks_JobId",
                table: "JobCancellationMarks",
                column: "JobId");
        }
    }
}
