using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaeRaces.Query.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class pilotclubdetailsaddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HomeLocationAddressLine1",
                table: "PilotClubDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeLocationAddressLine2",
                table: "PilotClubDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeLocationCity",
                table: "PilotClubDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeLocationCounty",
                table: "PilotClubDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeLocationPostcode",
                table: "PilotClubDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeLocationAddressLine1",
                table: "PilotClubDetails");

            migrationBuilder.DropColumn(
                name: "HomeLocationAddressLine2",
                table: "PilotClubDetails");

            migrationBuilder.DropColumn(
                name: "HomeLocationCity",
                table: "PilotClubDetails");

            migrationBuilder.DropColumn(
                name: "HomeLocationCounty",
                table: "PilotClubDetails");

            migrationBuilder.DropColumn(
                name: "HomeLocationPostcode",
                table: "PilotClubDetails");
        }
    }
}
