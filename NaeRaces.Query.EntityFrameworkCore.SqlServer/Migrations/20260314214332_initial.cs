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
                name: "ClubMemberRoles",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMemberRoles", x => new { x.ClubId, x.PilotId, x.Role });
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
                    IsRegistrationConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationValidatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegistrationValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubMembershipLevels",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipLevelId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PilotPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PolicyVersion = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMembershipLevels", x => new { x.ClubId, x.MembershipLevelId });
                });

            migrationBuilder.CreateTable(
                name: "ClubOverviews",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationPostcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationCounty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalMemberCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubOverviews", x => x.ClubId);
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
                name: "PilotClubDetails",
                columns: table => new
                {
                    PilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClubCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationPostcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeLocationCounty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MembershipLevelId = table.Column<int>(type: "int", nullable: true),
                    MembershipLevelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MembershipValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotClubDetails", x => new { x.PilotId, x.ClubId });
                });

            migrationBuilder.CreateTable(
                name: "PilotDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Callsign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PilotFollowedClubs",
                columns: table => new
                {
                    PilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotFollowedClubs", x => new { x.PilotId, x.ClubId });
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
                name: "PilotPolicyStatements",
                columns: table => new
                {
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementId = table.Column<int>(type: "int", nullable: false),
                    StatementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeftHandStatementId = table.Column<int>(type: "int", nullable: true),
                    Operand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RightHandStatementId = table.Column<int>(type: "int", nullable: true),
                    IsWithinBrackets = table.Column<bool>(type: "bit", nullable: true),
                    MinimumAge = table.Column<int>(type: "int", nullable: true),
                    MaximumAge = table.Column<int>(type: "int", nullable: true),
                    InsuranceProvider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GovernmentDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequiredMembershipLevelId = table.Column<int>(type: "int", nullable: true),
                    ValidationPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotPolicyStatements", x => new { x.PolicyId, x.StatementId });
                });

            migrationBuilder.CreateTable(
                name: "PilotRaceRegistrations",
                columns: table => new
                {
                    PilotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotRaceRegistrations", x => new { x.PilotId, x.RaceId });
                });

            migrationBuilder.CreateTable(
                name: "PilotSelectionPolicyDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestVersion = table.Column<long>(type: "bigint", nullable: false),
                    RootStatementId = table.Column<int>(type: "int", nullable: true)
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
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    PilotPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PilotPolicyVersion = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceDetails", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "RacePackages",
                columns: table => new
                {
                    RaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RacePackageId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApplyDiscounts = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationOpenDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationCloseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PilotPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PolicyVersion = table.Column<long>(type: "bigint", nullable: true),
                    IsRegistrationManuallyOpened = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RacePackages", x => new { x.RaceId, x.RacePackageId });
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

            migrationBuilder.CreateTable(
                name: "ClubMembershipLevelPaymentOption",
                columns: table => new
                {
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipLevelId = table.Column<int>(type: "int", nullable: false),
                    PaymentOptionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DayOfMonthDue = table.Column<int>(type: "int", nullable: true),
                    PaymentInterval = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubMembershipLevelPaymentOption", x => new { x.ClubId, x.MembershipLevelId, x.PaymentOptionId });
                    table.ForeignKey(
                        name: "FK_ClubMembershipLevelPaymentOption_ClubMembershipLevels_ClubId_MembershipLevelId",
                        columns: x => new { x.ClubId, x.MembershipLevelId },
                        principalTable: "ClubMembershipLevels",
                        principalColumns: new[] { "ClubId", "MembershipLevelId" },
                        onDelete: ReferentialAction.Cascade);
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
                name: "ClubMemberRoles");

            migrationBuilder.DropTable(
                name: "ClubMembers");

            migrationBuilder.DropTable(
                name: "ClubMembershipLevelPaymentOption");

            migrationBuilder.DropTable(
                name: "ClubOverviews");

            migrationBuilder.DropTable(
                name: "ClubUniquenessDetails");

            migrationBuilder.DropTable(
                name: "PilotAgeValidations");

            migrationBuilder.DropTable(
                name: "PilotClubDetails");

            migrationBuilder.DropTable(
                name: "PilotDetails");

            migrationBuilder.DropTable(
                name: "PilotFollowedClubs");

            migrationBuilder.DropTable(
                name: "PilotGovernmentDocumentValidations");

            migrationBuilder.DropTable(
                name: "PilotInsuranceProviderValidations");

            migrationBuilder.DropTable(
                name: "PilotPolicyStatements");

            migrationBuilder.DropTable(
                name: "PilotRaceRegistrations");

            migrationBuilder.DropTable(
                name: "PilotSelectionPolicyDetails");

            migrationBuilder.DropTable(
                name: "RaceDetails");

            migrationBuilder.DropTable(
                name: "RaceInformation");

            migrationBuilder.DropTable(
                name: "RacePackages");

            migrationBuilder.DropTable(
                name: "ReactionPositions");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "ClubMembershipLevels");
        }
    }
}
