using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatsReservation.Domain.Entities.Venues;
using SharedService.SharedKernel.BaseClasses;

namespace SeatsReservation.Infrastructure.Postgres.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("seats");
        
        builder.HasKey(s => s.Id)
               .HasName("pk_seats");
        
        builder.Property(s => s.Id)
               .HasConversion(
                   id => id.Value,
                   value => Id<Seat>.Create(value))
               .HasColumnName("id");
        
        builder.Property(s => s.SeatNumber)
               .IsRequired()
               .HasColumnName("seat_number");
        
        builder.Property(s => s.RowNumber)
               .IsRequired()
               .HasColumnName("row_number");
        

    }
}