using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatsReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class EventWithEventDetailsRelationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "capacity",
                schema: "seats_reservation",
                table: "events");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "seats_reservation",
                table: "events");

            migrationBuilder.DropColumn(
                name: "details_last_reservation",
                schema: "seats_reservation",
                table: "events");

            migrationBuilder.DropColumn(
                name: "details_version",
                schema: "seats_reservation",
                table: "events");

            migrationBuilder.AddForeignKey(
                name: "fk_event_details_events_event_id",
                schema: "seats_reservation",
                table: "event_details",
                column: "event_id",
                principalSchema: "seats_reservation",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_event_details_events_event_id",
                schema: "seats_reservation",
                table: "event_details");

            migrationBuilder.AddColumn<int>(
                name: "capacity",
                schema: "seats_reservation",
                table: "events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "seats_reservation",
                table: "events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "details_last_reservation",
                schema: "seats_reservation",
                table: "events",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "details_version",
                schema: "seats_reservation",
                table: "events",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
