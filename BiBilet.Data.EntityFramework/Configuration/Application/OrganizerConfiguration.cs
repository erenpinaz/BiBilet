using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Data.EntityFramework.Configuration.Application
{
    internal class OrganizerConfiguration : EntityTypeConfiguration<Organizer>
    {
        /// <summary>
        /// Fluent API configuration for the Organizer table
        /// </summary>
        internal OrganizerConfiguration()
        {
            ToTable("Organizer");

            HasKey(x => x.OrganizerId)
                .Property(x => x.OrganizerId)
                .HasColumnName("OrganizerId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar")
                .HasMaxLength(128)
                .IsRequired();

            Property(x => x.Description)
                .HasColumnName("Description")
                .HasColumnType("nvarchar")
                .IsMaxLength()
                .IsRequired();

            Property(x => x.Image)
                .HasColumnName("Image")
                .HasColumnType("nvarchar")
                .IsMaxLength()
                .IsRequired();

            Property(x => x.Website)
                .HasColumnName("Website")
                .HasColumnType("nvarchar")
                .HasMaxLength(256)
                .IsOptional();

            Property(x => x.Slug)
                .HasColumnName("Slug")
                .HasColumnType("nvarchar")
                .HasMaxLength(128)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Slug") {IsUnique = true}))
                .IsRequired();

            HasMany(x => x.Events)
                .WithRequired(y => y.Organizer)
                .HasForeignKey(y => y.OrganizerId);

            HasRequired(x => x.User)
                .WithMany(y => y.Organizers)
                .HasForeignKey(x => x.UserId);
        }
    }
}