using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodDiary.Core.NotificationAggregate;

namespace FoodDiary.Infrastructure.Data.Config;

public class NotificationPreferencesConfiguration : IEntityTypeConfiguration<NotificationPreferences>
{
    public void Configure(EntityTypeBuilder<NotificationPreferences> builder)
    {
        builder.ToTable("NotificationPreferences");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired();
            
        builder.Property(x => x.WaterRemindersEnabled)
            .IsRequired();
            
        builder.Property(x => x.MealRemindersEnabled)
            .IsRequired();
            
        builder.Property(x => x.CalorieLimitWarningsEnabled)
            .IsRequired();
            
        builder.Property(x => x.GoalAchievementsEnabled)
            .IsRequired();
            

            
        builder.Property(x => x.WeeklyProgressEnabled)
            .IsRequired();
            
        builder.Property(x => x.DailySummaryEnabled)
            .IsRequired();
            
        builder.Property(x => x.WaterReminderTime)
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(x => x.BreakfastReminderTime)
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(x => x.LunchReminderTime)
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(x => x.DinnerReminderTime)
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(x => x.WaterReminderFrequencyHours)
            .IsRequired();
            
        builder.Property(x => x.SendNotificationsOnWeekends)
            .IsRequired();
            
        builder.Property(x => x.CreatedAt)
            .IsRequired();
            
        builder.Property(x => x.UpdatedAt)
            .IsRequired();
            
        builder.HasOne(x => x.User)
            .WithOne(u => u.NotificationPreferences)
            .HasForeignKey<NotificationPreferences>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
} 