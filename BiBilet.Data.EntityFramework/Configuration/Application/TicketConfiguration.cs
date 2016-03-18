using System.Data.Entity.ModelConfiguration;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Data.EntityFramework.Configuration.Application
{
    internal class TicketConfiguration : EntityTypeConfiguration<Ticket>
    {
        /// <summary>
        /// Fluent API configuration for the Ticket table
        /// </summary>
        internal TicketConfiguration()
        {
            ToTable("Ticket");

            HasKey(x => x.TicketId)
                .Property(x => x.TicketId)
                .HasColumnName("TicketId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            Property(x => x.Title)
                .HasColumnName("Name")
                .HasColumnType("nvarchar")
                .HasMaxLength(128)
                .IsRequired();

            Property(x => x.Quantity)
                .HasColumnName("Quantity")
                .HasColumnType("int")
                .IsRequired();

            Property(x => x.Price)
                .HasColumnName("Price")
                .HasColumnType("decimal")
                .IsOptional();

            Property(x => x.Type)
                .HasColumnName("Type")
                .HasColumnType("int")
                .IsRequired();

            HasRequired(x => x.Event)
                .WithMany(y => y.Tickets)
                .HasForeignKey(x => x.EventId)
                .WillCascadeOnDelete(false);
        }
    }
}