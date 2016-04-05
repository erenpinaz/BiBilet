using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Data.EntityFramework.Configuration.Application
{
    internal class UserTicketConfiguration : EntityTypeConfiguration<UserTicket>
    {
        /// <summary>
        /// Fluent API configuration for the UserTicket table
        /// </summary>
        internal UserTicketConfiguration()
        {
            ToTable("UserTicket");

            HasKey(x => new
            {
                x.UserId,
                x.TicketId
            });

            Property(x => x.OwnerName)
                .HasColumnName("OwnerName")
                .HasColumnType("nvarchar")
                .HasMaxLength(128)
                .IsRequired();

            Property(x => x.OwnerAddress)
                .HasColumnName("OwnerAddress")
                .HasColumnType("nvarchar")
                .IsMaxLength()
                .IsRequired();

            Property(x => x.OwnerEmail)
                .HasColumnName("OwnerEmail")
                .HasColumnType("nvarchar")
                .HasMaxLength(320)
                .IsRequired();

            Property(x => x.OrderNumber)
                .HasColumnName("OrderNumber")
                .HasColumnType("nvarchar")
                .HasMaxLength(128)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_OrderNumber") {IsUnique = true}))
                .IsRequired();

            Property(x => x.OrderDate)
                .HasColumnName("OrderDate")
                .HasColumnType("datetime2")
                .IsRequired();

            HasRequired(x => x.User)
                .WithMany(x => x.UserTickets)
                .HasForeignKey(x => x.UserId);

            HasRequired(x => x.Ticket)
                .WithMany(x => x.UserTickets)
                .HasForeignKey(x => x.TicketId);
        }
    }
}