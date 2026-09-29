using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace F1Telemetry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    DriverNumber = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    TeamName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.DriverNumber);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionKey = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MeetingKey = table.Column<int>(type: "integer", nullable: false),
                    SessionName = table.Column<string>(type: "text", nullable: false),
                    DateStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.SessionKey);
                });

            migrationBuilder.CreateTable(
                name: "LapTimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionKey = table.Column<int>(type: "integer", nullable: false),
                    DriverNumber = table.Column<int>(type: "integer", nullable: false),
                    LapNumber = table.Column<int>(type: "integer", nullable: false),
                    LapDurationMs = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LapTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LapTimes_Drivers_DriverNumber",
                        column: x => x.DriverNumber,
                        principalTable: "Drivers",
                        principalColumn: "DriverNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LapTimes_Sessions_SessionKey",
                        column: x => x.SessionKey,
                        principalTable: "Sessions",
                        principalColumn: "SessionKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PitStops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionKey = table.Column<int>(type: "integer", nullable: false),
                    DriverNumber = table.Column<int>(type: "integer", nullable: false),
                    LapNumber = table.Column<int>(type: "integer", nullable: false),
                    PitDurationMs = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PitStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PitStops_Drivers_DriverNumber",
                        column: x => x.DriverNumber,
                        principalTable: "Drivers",
                        principalColumn: "DriverNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PitStops_Sessions_SessionKey",
                        column: x => x.SessionKey,
                        principalTable: "Sessions",
                        principalColumn: "SessionKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LapTimes_DriverNumber",
                table: "LapTimes",
                column: "DriverNumber");

            migrationBuilder.CreateIndex(
                name: "IX_LapTimes_SessionKey",
                table: "LapTimes",
                column: "SessionKey");

            migrationBuilder.CreateIndex(
                name: "IX_PitStops_DriverNumber",
                table: "PitStops",
                column: "DriverNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PitStops_SessionKey",
                table: "PitStops",
                column: "SessionKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LapTimes");

            migrationBuilder.DropTable(
                name: "PitStops");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
