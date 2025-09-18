using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Infrastructure.Data.Config;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.EmailConfirmationToken)
            .HasMaxLength(255);

        builder.Property(u => u.HeightCm)
            .HasPrecision(5, 2);

        builder.Property(u => u.WeightKg)
            .HasPrecision(5, 2);

        builder.Property(u => u.Age)
            .HasMaxLength(3);

        builder.Property(u => u.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(u => u.ActivityLevel)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.FitnessGoal)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.TargetWeightKg)
            .HasPrecision(5, 2);

        builder.Property(u => u.DailyCalorieGoal)
            .HasPrecision(8, 2);

        builder.Property(u => u.DailyProteinGoal)
            .HasPrecision(6, 2);

        builder.Property(u => u.DailyFatGoal)
            .HasPrecision(6, 2);

        builder.Property(u => u.DailyCarbohydrateGoal)
            .HasPrecision(6, 2);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.Name);
    }
} 