using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaeRaces.Query.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class cluboverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClubOverviews");
        }
    }
}
