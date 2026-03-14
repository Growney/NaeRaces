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
                name: "RaceInformation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstRaceDateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRaceDateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfRaceDates = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisteredPilotCount = table.Column<int>(type: "int", nullable: false),
                    MaximumPilots = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceInformation", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaceInformation");
        }
    }
}
