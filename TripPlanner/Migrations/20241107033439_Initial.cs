using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripPlanner.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    TripId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Accomodation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomodationEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThingsToDo1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThingsToDo2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThingsToDo3 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.TripId);
                });

            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "TripId", "Accomodation", "AccomodationEmail", "AccomodationPhone", "Destination", "EndDate", "StartDate", "ThingsToDo1", "ThingsToDo2", "ThingsToDo3" },
                values: new object[] { 1, "Banff Springs Hotel", "BanffSprings@Hotel.com", "403-403-4033", "Banff", new DateOnly(2024, 12, 19), new DateOnly(2024, 11, 11), "Ski", "Go to a resturant", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
