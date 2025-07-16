using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Infrastructure.Postgres.Configurations;

public class ReservationSeatConfiguration : IEntityTypeConfiguration<ReservationSeat>
{
    public void Configure(EntityTypeBuilder<ReservationSeat> builder)
    {
        builder.ToTable("reservation_seats");
        
        builder.HasKey(rs => rs.Id)
               .HasName("pk_reservation_seats");
        
        builder.Property(rs => rs.Id)
               .HasConversion(
                   id => id.Value,
                   value => Id<ReservationSeat>.Create(value))
               .HasColumnName("id");
        
        builder.HasOne(rs => rs.Reservation)
               .WithMany(r => r.ReservedSeats)
               .HasForeignKey("reservation_id")
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Seat>()
               .WithMany()
               .HasForeignKey(rs => rs.SeatId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(rs => rs.SeatId)
               .IsRequired()
               .HasConversion(
                      id => id.Value,
                      value => Id<Seat>.Create(value))
               .HasColumnName("seat_id");
        
        builder.Property(rs => rs.EventId)
               .IsRequired()
               .HasConversion(
                      id => id.Value,
                      value => Id<Event>.Create(value))
               .HasColumnName("event_id");
        
        // DateTimeOffset don't need conversion
        builder.Property(rs => rs.ReservationDate)
               .IsRequired()
               .HasColumnName("reservation_date");

        // уникальность для избежания повторного бронирования места
        builder.HasIndex(rs => new { rs.SeatId, rs.EventId })
               .IsUnique();
    }
}