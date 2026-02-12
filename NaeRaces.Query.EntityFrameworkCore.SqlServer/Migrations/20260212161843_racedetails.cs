using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaeRaces.Query.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class racedetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RaceDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTeamRace = table.Column<bool>(type: "bit", nullable: false),
                    FirstRaceDateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRaceDateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfRaceDates = table.Column<int>(type: "int", nullable: false),
                    IsDetailsPublished = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceDetails", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaceDetails");
        }
    }
}
