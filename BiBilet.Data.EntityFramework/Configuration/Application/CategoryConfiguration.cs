using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using BiBilet.Domain.Entities.Application;

namespace BiBilet.Data.EntityFramework.Configuration.Application
{
    internal class CategoryConfiguration : EntityTypeConfiguration<Category>
    {
        /// <summary>
        /// Fluent API configuration for the Category table
        /// </summary>
        internal CategoryConfiguration()
        {
            ToTable("Category");

            HasKey(x => x.CategoryId)
                .Property(x => x.CategoryId)
                .HasColumnName("CategoryId")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("Name")
                .HasColumnType("nvarchar")
                .HasMaxLength(64)
                .IsRequired();

            Property(x => x.Slug)
                .HasColumnName("Slug")
                .HasColumnType("nvarchar")
                .HasMaxLength(64)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Slug") {IsUnique = true}))
                .IsRequired();

            HasMany(x => x.Events)
                .WithRequired(y => y.Category)
                .HasForeignKey(y => y.CategoryId);
        }
    }
}