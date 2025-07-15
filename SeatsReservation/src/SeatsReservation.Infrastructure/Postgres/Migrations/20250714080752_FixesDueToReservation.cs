using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatsReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class FixesDueToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ended_at",
                schema: "seats_reservation",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "started_at",
                schema: "seats_reservation",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "status",
                schema: "seats_reservation",
                table: "events",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ended_at",
                schema: "seats_reservation",
                table: "events");

            migrationBuilder.DropColumn(
                name: "started_at",
                schema: "seats_reservation",
                table: "events");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "seats_reservation",
                table: "events");
        }
    }
}
