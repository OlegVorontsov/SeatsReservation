using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Domain.Shared;
using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Infrastructure.Postgres.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");
        
        builder.HasKey(v => v.Id)
               .HasName("pk_venues");
        
        builder.Property(v => v.Id)
               .HasConversion(
                   id => id.Value,
                   value => Id<Venue>.Create(value))
               .HasColumnName("id");
        
        builder.ComplexProperty(v => v.Name, vnb =>
        {
            vnb.Property(n => n.Name)
                .IsRequired()
                .HasMaxLength(Constants.Length._500)
                .HasColumnName("name");

            vnb.Property(p => p.Prefix)
                .IsRequired()
                .HasMaxLength(Constants.Length._50)
                .HasColumnName("prefix");
        });
        
        builder.Property(v => v.SeatsLimit)
               .IsRequired()
               .HasColumnName("seats_limit");

        builder.HasMany(v => v.Seats)
               .WithOne(s => s.Venue)
               .HasForeignKey("venue_id")
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

    }
}