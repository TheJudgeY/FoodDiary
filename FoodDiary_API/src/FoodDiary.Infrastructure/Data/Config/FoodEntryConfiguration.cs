using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.Infrastructure.Data.Config;

public class FoodEntryConfiguration : IEntityTypeConfiguration<FoodEntry>
{
    public void Configure(EntityTypeBuilder<FoodEntry> builder)
    {
        builder.ToTable("FoodEntries");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired();
            
        builder.Property(x => x.ProductId)
            .IsRequired();
            
        builder.Property(x => x.WeightGrams)
            .HasPrecision(8, 2)
            .IsRequired();
            
        builder.Property(x => x.MealType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
            
        builder.Property(x => x.ConsumedAt)
            .IsRequired();
            
        builder.Property(x => x.Notes)
            .HasMaxLength(500);
            
        builder.HasOne(x => x.User)
            .WithMany(u => u.FoodEntries)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ConsumedAt);
        builder.HasIndex(x => new { x.UserId, x.ConsumedAt });
    }
} 