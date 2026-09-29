using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Telemetry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTireDataToLapTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TireAge",
                table: "LapTimes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TireCompound",
                table: "LapTimes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TireAge",
                table: "LapTimes");

            migrationBuilder.DropColumn(
                name: "TireCompound",
                table: "LapTimes");
        }
    }
}
