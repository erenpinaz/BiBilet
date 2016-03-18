using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Data.EntityFramework.Configuration.Application
{
    internal class EventConfiguration : EntityTypeConfiguration<Event>
    {
        /// <summary>
        /// Fluent API configuration for the Event table
        /// </summary>
        internal EventConfiguration()
        {
            ToTable("Event");

            HasKey(x => x.EventId)
                .Property(x => x.EventId)
                .HasColumnName("EventId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            Property(x => x.Title)
                .HasColumnName("Title")
                .HasColumnType("nvarchar")
                .HasMaxLength(1024)
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

            Property(x => x.Slug)
                .HasColumnName("Slug")
                .HasColumnType("nvarchar")
                .HasMaxLength(1024)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Slug") {IsUnique = true}))
                .IsRequired();

            Property(x => x.Published)
                .HasColumnName("Published")
                .HasColumnType("bit")
                .IsRequired();

            Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .HasColumnType("datetime2")
                .IsRequired();

            Property(x => x.EndDate)
                .HasColumnName("EndDate")
                .HasColumnType("datetime2")
                .IsRequired();

            HasRequired(x => x.Category)
                .WithMany(y => y.Events)
                .HasForeignKey(x => x.CategoryId);

            HasRequired(x => x.Topic)
                .WithMany(y => y.Events)
                .HasForeignKey(x => x.TopicId);

            HasRequired(x => x.SubTopic)
                .WithMany(y => y.Events)
                .HasForeignKey(x => x.SubTopicId);

            HasRequired(x => x.Venue)
                .WithMany(y => y.Events)
                .HasForeignKey(x => x.VenueId);

            HasMany(x => x.Tickets)
                .WithRequired(y => y.Event)
                .HasForeignKey(x => x.EventId)
                .WillCascadeOnDelete(false);
        }
    }
}