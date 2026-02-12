using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaeRaces.Query.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class pilotpeervalidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "PilotDetails",
                type: "datetime2",
                nullable: true);

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
                name: "RacePolicyDetails",
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
                    table.PrimaryKey("PK_RacePolicyDetails", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PilotAgeValidations");

            migrationBuilder.DropTable(
                name: "PilotGovernmentDocumentValidations");

            migrationBuilder.DropTable(
                name: "PilotInsuranceProviderValidations");

            migrationBuilder.DropTable(
                name: "RacePolicyDetails");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "PilotDetails");
        }
    }
}
