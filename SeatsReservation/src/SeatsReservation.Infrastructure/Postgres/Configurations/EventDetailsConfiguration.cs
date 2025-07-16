using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Infrastructure.Postgres.Converters;
using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Infrastructure.Postgres.Configurations;

public class EventDetailsConfiguration : IEntityTypeConfiguration<EventDetails>
{
    public void Configure(EntityTypeBuilder<EventDetails> builder)
    {
        builder.HasKey(ed => ed.EventId);
            
        builder.Property(ed => ed.Version)
               .IsRowVersion();
    }
}