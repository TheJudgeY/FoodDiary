using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.Infrastructure.Data.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.CaloriesPer100g)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.Property(p => p.ProteinsPer100g)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.Property(p => p.FatsPer100g)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.Property(p => p.CarbohydratesPer100g)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.Property(p => p.Description)
            .HasMaxLength(500);
            
        builder.Property(p => p.Category)
            .HasConversion<string>()
            .HasMaxLength(50);
            
        builder.Property(p => p.ImageData)
            .HasColumnType("bytea");
            
        builder.Property(p => p.ImageContentType)
            .HasMaxLength(100);
            
        builder.Property(p => p.ImageFileName)
            .HasMaxLength(255);

        builder.HasIndex(p => p.Name);

        builder.HasData(
            Product.CreateForSeeding(
                Guid.Parse("12345678-1234-1234-1234-123456789001"),
                "Chicken Breast",
                165,
                31.0,
                3.6,
                0.0,
                "Lean chicken breast, skinless and boneless"
            ),
            Product.CreateForSeeding(
                Guid.Parse("12345678-1234-1234-1234-123456789002"),
                "Brown Rice",
                111,
                2.6,
                0.9,
                23.0,
                "Cooked brown rice"
            ),
            Product.CreateForSeeding(
                Guid.Parse("12345678-1234-1234-1234-123456789003"),
                "Broccoli",
                34,
                2.8,
                0.4,
                7.0,
                "Fresh broccoli florets"
            ),
            Product.CreateForSeeding(
                Guid.Parse("12345678-1234-1234-1234-123456789004"),
                "Salmon",
                208,
                25.0,
                12.0,
                0.0,
                "Atlantic salmon fillet"
            ),
            Product.CreateForSeeding(
                Guid.Parse("12345678-1234-1234-1234-123456789005"),
                "Sweet Potato",
                86,
                1.6,
                0.1,
                20.0,
                "Baked sweet potato"
            )
        );
    }
} 