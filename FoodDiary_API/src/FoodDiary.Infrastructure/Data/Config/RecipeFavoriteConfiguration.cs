using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.Infrastructure.Data.Config;

public class RecipeFavoriteConfiguration : IEntityTypeConfiguration<RecipeFavorite>
{
    public void Configure(EntityTypeBuilder<RecipeFavorite> builder)
    {
        builder.HasKey(rf => rf.Id);

        builder.Property(rf => rf.Id)
            .IsRequired();

        builder.Property(rf => rf.UserId)
            .IsRequired();

        builder.Property(rf => rf.RecipeId)
            .IsRequired();

        builder.Property(rf => rf.RelationshipType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(rf => rf.CreatedAt)
            .IsRequired();

        builder.HasOne(rf => rf.User)
            .WithMany()
            .HasForeignKey(rf => rf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rf => rf.Recipe)
            .WithMany(r => r.Favorites)
            .HasForeignKey(rf => rf.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rf => new { rf.UserId, rf.RecipeId, rf.RelationshipType })
            .IsUnique();

        builder.HasIndex(rf => rf.UserId);
        builder.HasIndex(rf => rf.RecipeId);
        builder.HasIndex(rf => rf.CreatedAt);
    }
}
