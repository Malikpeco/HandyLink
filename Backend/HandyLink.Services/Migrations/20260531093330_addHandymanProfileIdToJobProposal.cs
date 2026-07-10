using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandyLink.Services.Migrations
{
    /// <inheritdoc />
    public partial class addHandymanProfileIdToJobProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HandymanProfileId",
                table: "JobProposals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobProposals_HandymanProfileId",
                table: "JobProposals",
                column: "HandymanProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobProposals_HandymanProfiles_HandymanProfileId",
                table: "JobProposals",
                column: "HandymanProfileId",
                principalTable: "HandymanProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobProposals_HandymanProfiles_HandymanProfileId",
                table: "JobProposals");

            migrationBuilder.DropIndex(
                name: "IX_JobProposals_HandymanProfileId",
                table: "JobProposals");

            migrationBuilder.DropColumn(
                name: "HandymanProfileId",
                table: "JobProposals");
        }
    }
}
