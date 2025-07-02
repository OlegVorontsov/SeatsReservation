using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Infrastructure.Postgres.Converters;
using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Infrastructure.Postgres.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");
        
        builder.HasKey(e => e.Id)
               .HasName("pk_events");
        
        builder.Property(e => e.Id)
               .HasConversion(
                   id => id.Value,
                   value => Id<Event>.Create(value))
               .HasColumnName("id");
        
        builder.Property(e => e.Name)
               .IsRequired()
               .HasColumnName("name");
        
        // DateTimeOffset don't need conversion
        builder.Property(e => e.EventDate)
               .IsRequired()
               .HasColumnName("event_date");
        
        builder.ComplexProperty(e => e.Details, db =>
        {
            db.Property(d => d.Capacity)
                .IsRequired()
                .HasColumnName("capacity");

            db.Property(d => d.Description)
                .IsRequired(false)
                .HasColumnName("description");
        });
        
        builder.HasOne<Venue>()
               .WithMany()
               .HasForeignKey(v => v.VenueId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(e => e.VenueId)
               .HasColumnName("venue_id");
        
        builder.Property(e => e.EventType)
               .HasConversion<string>()
               .HasColumnName("event_type");
        
        builder.Property(e => e.EventInfo)
               .HasConversion(new EventInfoConverter())
               .HasColumnName("event_info");
    }
}