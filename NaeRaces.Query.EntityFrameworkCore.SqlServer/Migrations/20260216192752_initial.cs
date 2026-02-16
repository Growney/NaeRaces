using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaeRaces.Query.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClubDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FounderPilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubLocations",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    County = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimezoneOffsetMinutes = table.Column<int>(type: "int", nullable: true),
                    UseDaylightSavings = table.Column<bool>(type: "bit", nullable: true),
                    IsHomeLocation = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubLocations", x => new { x.ClubId, x.LocationId });
                });

            migrationBuilder.CreateTable(
                name: "ClubMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipLevelId = table.Column<int>(type: "int", nullable: true),
                    PaymentOptionId = table.Column<int>(type: "int", nullable: true),
                    IsOnCommittee = table.Column<bool>(type: "bit", nullable: false),
                    IsRegistrationConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationValidatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegistrationValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubUniquenessDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubUniquenessDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PilotAgeValidations",
                columns: table => new
                {
                    PilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidatedByPilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidatedByClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsOnClubCommittee = table.Column<bool>(type: "bit", nullable: false),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotAgeValidations", x => new { x.PilotId, x.ValidatedByPilotId, x.ValidatedByClubId });
                });

            migrationBuilder.CreateTable(
                name: "PilotDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Callsign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PilotGovernmentDocumentValidations",
                columns: table => new
                {
                    PilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GovernmentDocument = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ValidatedByPilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidatedByClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsOnClubCommittee = table.Column<bool>(type: "bit", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotGovernmentDocumentValidations", x => new { x.PilotId, x.GovernmentDocument, x.ValidatedByPilotId, x.ValidatedByClubId });
                });

            migrationBuilder.CreateTable(
                name: "PilotInsuranceProviderValidations",
                columns: table => new
                {
                    PilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsuranceProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ValidatedByPilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidatedByClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsOnClubCommittee = table.Column<bool>(type: "bit", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotInsuranceProviderValidations", x => new { x.PilotId, x.InsuranceProvider, x.ValidatedByPilotId, x.ValidatedByClubId });
                });

            migrationBuilder.CreateTable(
                name: "PilotSelectionPolicyDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotSelectionPolicyDetails", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "ReactionPositions",
                columns: table => new
                {
                    ReactionKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GlobalPosition = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionPositions", x => x.ReactionKey);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => new { x.TeamId, x.PilotId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubDetails");

            migrationBuilder.DropTable(
                name: "ClubLocations");

            migrationBuilder.DropTable(
                name: "ClubMembers");

            migrationBuilder.DropTable(
                name: "ClubUniquenessDetails");

            migrationBuilder.DropTable(
                name: "PilotAgeValidations");

            migrationBuilder.DropTable(
                name: "PilotDetails");

            migrationBuilder.DropTable(
                name: "PilotGovernmentDocumentValidations");

            migrationBuilder.DropTable(
                name: "PilotInsuranceProviderValidations");

            migrationBuilder.DropTable(
                name: "PilotSelectionPolicyDetails");

            migrationBuilder.DropTable(
                name: "RaceDetails");

            migrationBuilder.DropTable(
                name: "ReactionPositions");

            migrationBuilder.DropTable(
                name: "TeamMembers");
        }
    }
}
