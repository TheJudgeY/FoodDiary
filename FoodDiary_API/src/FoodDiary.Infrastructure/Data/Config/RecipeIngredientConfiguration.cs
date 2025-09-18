using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.Infrastructure.Data.Config;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.QuantityGrams)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.Property(ri => ri.Notes)
            .HasMaxLength(500);

        builder.Property(ri => ri.CustomIngredientName)
            .HasMaxLength(100);

        builder.Property(ri => ri.CustomCaloriesPer100g)
            .HasPrecision(8, 2);

        builder.Property(ri => ri.CustomProteinPer100g)
            .HasPrecision(8, 2);

        builder.Property(ri => ri.CustomFatPer100g)
            .HasPrecision(8, 2);

        builder.Property(ri => ri.CustomCarbohydratesPer100g)
            .HasPrecision(8, 2);

        builder.HasOne(ri => ri.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.Product)
            .WithMany()
            .HasForeignKey(ri => ri.ProductId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasIndex(ri => ri.RecipeId);
        builder.HasIndex(ri => ri.ProductId);
    }
}
