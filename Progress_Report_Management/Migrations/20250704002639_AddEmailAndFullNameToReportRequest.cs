using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressReportSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailAndFullNameToReportRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ReportRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ReportRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "ReportRequests");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ReportRequests");
        }
    }
}
