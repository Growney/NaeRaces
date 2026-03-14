using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaeRaces.Query.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class pilotclubdetailsmembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MembershipLevelId",
                table: "PilotClubDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MembershipLevelName",
                table: "PilotClubDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MembershipValidUntil",
                table: "PilotClubDetails",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MembershipLevelId",
                table: "PilotClubDetails");

            migrationBuilder.DropColumn(
                name: "MembershipLevelName",
                table: "PilotClubDetails");

            migrationBuilder.DropColumn(
                name: "MembershipValidUntil",
                table: "PilotClubDetails");
        }
    }
}
