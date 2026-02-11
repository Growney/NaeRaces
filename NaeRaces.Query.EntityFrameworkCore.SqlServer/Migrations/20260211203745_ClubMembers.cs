using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaeRaces.Query.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ClubMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubMembers");
        }
    }
}
