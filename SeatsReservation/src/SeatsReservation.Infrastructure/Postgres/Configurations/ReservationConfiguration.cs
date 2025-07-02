using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Infrastructure.Postgres.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");
        
        builder.HasKey(e => e.Id)
               .HasName("pk_reservations");
        
        builder.Property(e => e.Id)
               .HasConversion(
                   id => id.Value,
                   value => Id<Reservation>.Create(value))
               .HasColumnName("id");
        
        builder.Property(v => v.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasColumnName("reservation_status");
        
        builder.Property(e => e.EventId)
               .HasConversion(
                   id => id.Value,
                   value => Id<Event>.Create(value))
               .HasColumnName("event_id");
        
        builder.Property(e => e.UserId)
               .IsRequired()
               .HasColumnName("user_id");
        
        // DateTimeOffset don't need conversion
        builder.Property(e => e.CreatedAt)
               .IsRequired()
               .HasColumnName("created_at");
    }
}