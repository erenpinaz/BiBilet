using System.Data.Entity.ModelConfiguration;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Data.EntityFramework.Configuration.Application
{
    internal class TopicConfiguration : EntityTypeConfiguration<Topic>
    {
        /// <summary>
        /// Fluent API configuration for the Topic table
        /// </summary>
        internal TopicConfiguration()
        {
            ToTable("Topic");

            HasKey(x => x.TopicId)
                .Property(x => x.TopicId)
                .HasColumnName("TopicId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar")
                .HasMaxLength(64)
                .IsRequired();

            HasMany(x => x.Events)
                .WithRequired(y => y.Topic)
                .HasForeignKey(y => y.TopicId)
                .WillCascadeOnDelete(false);
        }
    }

    internal class SubTopicConfiguration : EntityTypeConfiguration<SubTopic>
    {
        /// <summary>
        /// Fluent API configuration for the SubTopic table
        /// </summary>
        internal SubTopicConfiguration()
        {
            ToTable("SubTopic");

            HasKey(x => x.SubTopicId)
                .Property(x => x.SubTopicId)
                .HasColumnName("SubTopicId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar")
                .HasMaxLength(64)
                .IsRequired();

            HasMany(x => x.Events)
                .WithRequired(y => y.SubTopic)
                .HasForeignKey(y => y.SubTopicId);

            HasRequired(x => x.Topic)
                .WithMany(y => y.SubTopics)
                .HasForeignKey(x => x.TopicId)
                .WillCascadeOnDelete(false);
        }
    }
}