using System.Data.Entity.ModelConfiguration;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Data.EntityFramework.Configuration.Application
{
    internal class VenueConfiguration : EntityTypeConfiguration<Venue>
    {
        /// <summary>
        /// Fluent API configuration for the Venue table
        /// </summary>
        internal VenueConfiguration()
        {
            ToTable("Venue");

            HasKey(x => x.VenueId)
                .Property(x => x.VenueId)
                .HasColumnName("SubTopicId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar")
                .HasMaxLength(256)
                .IsRequired();

            Property(x => x.Address)
                .HasColumnName("Address")
                .HasColumnType("nvarchar")
                .IsMaxLength()
                .IsRequired();

            Property(x => x.City)
                .HasColumnName("City")
                .HasColumnType("nvarchar")
                .HasMaxLength(64)
                .IsRequired();

            Property(x => x.Country)
                .HasColumnName("Country")
                .HasColumnType("nvarchar")
                .HasMaxLength(64)
                .IsRequired();

            HasMany(x => x.Events)
                .WithRequired(y => y.Venue)
                .HasForeignKey(y => y.VenueId);
        }
    }
}