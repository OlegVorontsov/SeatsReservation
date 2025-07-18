using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatsReservation.Domain.Entities.Users;
using SeatsReservation.Domain.Shared;

namespace SeatsReservation.Infrastructure.Postgres.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id).HasName("pk_users");

        builder.Property(u => u.Id)
            .HasColumnName("id");

        builder.OwnsOne(u => u.Details, db =>
        {
            db.ToJson("details");

            db.OwnsMany(d => d.Socials, sb =>
            {
                sb.Property(u => u.Link)
                    .IsRequired()
                    .HasMaxLength(Constants.Length._500)
                    .HasColumnName("link");
                
                sb.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(Constants.Length._500)
                    .HasColumnName("name");
            });

            db.Property(u => u.Description)
                .IsRequired()
                .HasMaxLength(Constants.Length._500)
                .HasColumnName("description");
        });
    }
}