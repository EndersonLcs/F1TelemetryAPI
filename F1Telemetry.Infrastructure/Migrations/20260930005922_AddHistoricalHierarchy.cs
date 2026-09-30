using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace F1Telemetry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricalHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Year = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Year);
                });

            migrationBuilder.CreateTable(
                name: "GrandPrixes",
                columns: table => new
                {
                    MeetingKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeasonYear = table.Column<int>(type: "integer", nullable: false),
                    RoundNumber = table.Column<int>(type: "integer", nullable: false),
                    CountryName = table.Column<string>(type: "text", nullable: false),
                    CircuitShortName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrandPrixes", x => x.MeetingKey);
                    table.ForeignKey(
                        name: "FK_GrandPrixes_Seasons_SeasonYear",
                        column: x => x.SeasonYear,
                        principalTable: "Seasons",
                        principalColumn: "Year",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_MeetingKey",
                table: "Sessions",
                column: "MeetingKey");

            migrationBuilder.CreateIndex(
                name: "IX_GrandPrixes_SeasonYear",
                table: "GrandPrixes",
                column: "SeasonYear");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_GrandPrixes_MeetingKey",
                table: "Sessions",
                column: "MeetingKey",
                principalTable: "GrandPrixes",
                principalColumn: "MeetingKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_GrandPrixes_MeetingKey",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "GrandPrixes");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_MeetingKey",
                table: "Sessions");
        }
    }
}
