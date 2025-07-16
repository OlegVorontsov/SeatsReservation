using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatsReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexSeatIdEventId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_reservation_seats_seat_id",
                schema: "seats_reservation",
                table: "reservation_seats");

            migrationBuilder.AddColumn<Guid>(
                name: "event_id",
                schema: "seats_reservation",
                table: "reservation_seats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_reservation_seats_seat_id_event_id",
                schema: "seats_reservation",
                table: "reservation_seats",
                columns: new[] { "seat_id", "event_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_reservation_seats_seat_id_event_id",
                schema: "seats_reservation",
                table: "reservation_seats");

            migrationBuilder.DropColumn(
                name: "event_id",
                schema: "seats_reservation",
                table: "reservation_seats");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_seats_seat_id",
                schema: "seats_reservation",
                table: "reservation_seats",
                column: "seat_id");
        }
    }
}
