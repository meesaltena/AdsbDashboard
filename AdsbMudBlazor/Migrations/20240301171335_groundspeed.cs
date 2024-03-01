using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdsbMudBlazor.Migrations
{
    /// <inheritdoc />
    public partial class groundspeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flights_ModeS_Callsign_Alt_Squawk_Lat_Long_DateTime",
                table: "Flights");

            migrationBuilder.AddColumn<int>(
                name: "Groundspeed",
                table: "Flights",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Groundspeed",
                table: "Flights");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ModeS_Callsign_Alt_Squawk_Lat_Long_DateTime",
                table: "Flights",
                columns: new[] { "ModeS", "Callsign", "Alt", "Squawk", "Lat", "Long", "DateTime" },
                unique: true);
        }
    }
}
