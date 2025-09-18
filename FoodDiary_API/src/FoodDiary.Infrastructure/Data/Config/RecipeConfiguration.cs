using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.Infrastructure.Data.Config;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Category)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.Servings)
            .IsRequired();

        builder.Property(r => r.PreparationTimeMinutes)
            .IsRequired();

        builder.Property(r => r.CookingTimeMinutes)
            .IsRequired();

        builder.Property(r => r.Instructions)
            .IsRequired()
            .HasMaxLength(2000);


            
        builder.Property(r => r.ImageData)
            .HasColumnType("bytea");
            
        builder.Property(r => r.ImageContentType)
            .HasMaxLength(100);
            
        builder.Property(r => r.ImageFileName)
            .HasMaxLength(255);

        builder.Property(r => r.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.TotalCalories)
            .HasPrecision(10, 2)
            .HasDefaultValue(0);

        builder.Property(r => r.TotalProtein)
            .HasPrecision(10, 2)
            .HasDefaultValue(0);

        builder.Property(r => r.TotalFat)
            .HasPrecision(10, 2)
            .HasDefaultValue(0);

        builder.Property(r => r.TotalCarbohydrates)
            .HasPrecision(10, 2)
            .HasDefaultValue(0);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        builder.HasMany(r => r.Favorites)
            .WithOne(rf => rf.Recipe)
            .HasForeignKey(rf => rf.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Ingredients)
            .WithOne(ri => ri.Recipe)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.Category);
        builder.HasIndex(r => r.IsPublic);
    }
} 