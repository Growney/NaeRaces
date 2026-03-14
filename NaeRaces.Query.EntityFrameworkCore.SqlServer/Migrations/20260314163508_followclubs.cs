using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaeRaces.Query.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class followclubs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnCommittee",
                table: "ClubMembers");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubMemberRoles");

            migrationBuilder.DropTable(
                name: "PilotFollowedClubs");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnCommittee",
                table: "ClubMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
