using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficIncidentsOpitech.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrafficIncidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IncidentType = table.Column<int>(type: "int", nullable: false),
                    VictimCount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficIncidents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrafficIncidentVehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    Plate = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrafficIncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficIncidentVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrafficIncidentVehicles_TrafficIncidents_TrafficIncidentId",
                        column: x => x.TrafficIncidentId,
                        principalTable: "TrafficIncidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrafficIncidents_Department",
                table: "TrafficIncidents",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficIncidents_Department_OccurredAt",
                table: "TrafficIncidents",
                columns: new[] { "Department", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TrafficIncidents_OccurredAt",
                table: "TrafficIncidents",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficIncidentVehicles_TrafficIncidentId",
                table: "TrafficIncidentVehicles",
                column: "TrafficIncidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrafficIncidentVehicles");

            migrationBuilder.DropTable(
                name: "TrafficIncidents");
        }
    }
}
