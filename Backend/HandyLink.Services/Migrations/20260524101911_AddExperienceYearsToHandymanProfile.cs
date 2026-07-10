using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandyLink.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddExperienceYearsToHandymanProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExperienceYears",
                table: "HandymanProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "User can use the application normally as a client or handyman.");

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "User is waiting for handyman application approval.");

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Users handyman application was rejected.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceYears",
                table: "HandymanProfiles");

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "User can use the application normally.");

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "User is waiting for approval.");

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "User/application was rejected.");
        }
    }
}
