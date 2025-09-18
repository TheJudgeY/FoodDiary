
using System;
using FoodDiary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FoodDiary.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250807152015_RemoveImageUrlFromRecipes")]
    partial class RemoveImageUrlFromRecipes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FoodDiary.Core.FoodEntryAggregate.FoodEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ConsumedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MealType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<double>("WeightGrams")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("ConsumedAt");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId", "ConsumedAt");

                    b.ToTable("FoodEntries", (string)null);
                });

            modelBuilder.Entity("FoodDiary.Core.NotificationAggregate.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ActionUrl")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DismissedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("Priority")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<DateTime?>("ReadAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Status");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId", "CreatedAt");

                    b.HasIndex("UserId", "Status");

                    b.ToTable("Notifications", (string)null);
                });

            modelBuilder.Entity("FoodDiary.Core.NotificationAggregate.NotificationPreferences", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("BreakfastReminderTime")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<bool>("CalorieLimitWarningsEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("DailySummaryEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("DinnerReminderTime")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<bool>("GoalAchievementsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("LunchReminderTime")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<bool>("MealRemindersEnabled")
                        .HasColumnType("boolean");

                    b.Property<bool>("SendNotificationsOnWeekends")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShoppingRemindersEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("WaterReminderFrequencyHours")
                        .HasColumnType("integer");

                    b.Property<string>("WaterReminderTime")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<bool>("WaterRemindersEnabled")
                        .HasColumnType("boolean");

                    b.Property<bool>("WeeklyProgressEnabled")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("NotificationPreferences", (string)null);
                });

            modelBuilder.Entity("FoodDiary.Core.ProductAggregate.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("CaloriesPer100g")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.Property<double>("CarbohydratesPer100g")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<double>("FatsPer100g")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.Property<string>("ImageContentType")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<byte[]>("ImageData")
                        .HasColumnType("bytea");

                    b.Property<string>("ImageFileName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<double>("ProteinsPer100g")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = new Guid("12345678-1234-1234-1234-123456789001"),
                            CaloriesPer100g = 165.0,
                            CarbohydratesPer100g = 0.0,
                            Category = "Other",
                            Description = "Lean chicken breast, skinless and boneless",
                            FatsPer100g = 3.6000000000000001,
                            Name = "Chicken Breast",
                            ProteinsPer100g = 31.0
                        },
                        new
                        {
                            Id = new Guid("12345678-1234-1234-1234-123456789002"),
                            CaloriesPer100g = 111.0,
                            CarbohydratesPer100g = 23.0,
                            Category = "Other",
                            Description = "Cooked brown rice",
                            FatsPer100g = 0.90000000000000002,
                            Name = "Brown Rice",
                            ProteinsPer100g = 2.6000000000000001
                        },
                        new
                        {
                            Id = new Guid("12345678-1234-1234-1234-123456789003"),
                            CaloriesPer100g = 34.0,
                            CarbohydratesPer100g = 7.0,
                            Category = "Other",
                            Description = "Fresh broccoli florets",
                            FatsPer100g = 0.40000000000000002,
                            Name = "Broccoli",
                            ProteinsPer100g = 2.7999999999999998
                        },
                        new
                        {
                            Id = new Guid("12345678-1234-1234-1234-123456789004"),
                            CaloriesPer100g = 208.0,
                            CarbohydratesPer100g = 0.0,
                            Category = "Other",
                            Description = "Atlantic salmon fillet",
                            FatsPer100g = 12.0,
                            Name = "Salmon",
                            ProteinsPer100g = 25.0
                        },
                        new
                        {
                            Id = new Guid("12345678-1234-1234-1234-123456789005"),
                            CaloriesPer100g = 86.0,
                            CarbohydratesPer100g = 20.0,
                            Category = "Other",
                            Description = "Baked sweet potato",
                            FatsPer100g = 0.10000000000000001,
                            Name = "Sweet Potato",
                            ProteinsPer100g = 1.6000000000000001
                        });
                });

            modelBuilder.Entity("FoodDiary.Core.RecipeAggregate.Recipe", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CookingTimeMinutes")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("ImageContentType")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<byte[]>("ImageData")
                        .HasColumnType("bytea");

                    b.Property<string>("ImageFileName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Instructions")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<bool>("IsFavorite")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsPublic")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("PreparationTimeMinutes")
                        .HasColumnType("integer");

                    b.Property<int>("Servings")
                        .HasColumnType("integer");

                    b.Property<double>("TotalCalories")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 2)
                        .HasColumnType("double precision")
                        .HasDefaultValue(0.0);

                    b.Property<double>("TotalCarbohydrates")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 2)
                        .HasColumnType("double precision")
                        .HasDefaultValue(0.0);

                    b.Property<double>("TotalFat")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 2)
                        .HasColumnType("double precision")
                        .HasDefaultValue(0.0);

                    b.Property<double>("TotalProtein")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 2)
                        .HasColumnType("double precision")
                        .HasDefaultValue(0.0);

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Category");

                    b.HasIndex("IsFavorite");

                    b.HasIndex("IsPublic");

                    b.HasIndex("UserId");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("FoodDiary.Core.RecipeAggregate.RecipeIngredient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double?>("CustomCaloriesPer100g")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.Property<double?>("CustomCarbohydratesPer100g")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.Property<double?>("CustomFatPer100g")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.Property<string>("CustomIngredientName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<double?>("CustomProteinPer100g")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<double>("QuantityGrams")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("RecipeId");

                    b.ToTable("RecipeIngredients");
                });

            modelBuilder.Entity("FoodDiary.Core.UserAggregate.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int?>("Age")
                        .HasMaxLength(3)
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("DailyCalorieGoal")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

                    b.Property<double?>("DailyCarbohydrateGoal")
                        .HasPrecision(6, 2)
                        .HasColumnType("double precision");

                    b.Property<double?>("DailyFatGoal")
                        .HasPrecision(6, 2)
                        .HasColumnType("double precision");

                    b.Property<double?>("DailyProteinGoal")
                        .HasPrecision(6, 2)
                        .HasColumnType("double precision");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("EmailConfirmationToken")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("EmailConfirmationTokenExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<double?>("HeightCm")
                        .HasPrecision(5, 2)
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<double?>("TargetWeightKg")
                        .HasPrecision(5, 2)
                        .HasColumnType("double precision");

                    b.Property<string>("TimeZoneId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ActivityLevel")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("FitnessGoal")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<double?>("WeightKg")
                        .HasPrecision(5, 2)
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FoodDiary.Core.FoodEntryAggregate.FoodEntry", b =>
                {
                    b.HasOne("FoodDiary.Core.ProductAggregate.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FoodDiary.Core.UserAggregate.User", "User")
                        .WithMany("FoodEntries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FoodDiary.Core.NotificationAggregate.Notification", b =>
                {
                    b.HasOne("FoodDiary.Core.UserAggregate.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FoodDiary.Core.NotificationAggregate.NotificationPreferences", b =>
                {
                    b.HasOne("FoodDiary.Core.UserAggregate.User", "User")
                        .WithOne("NotificationPreferences")
                        .HasForeignKey("FoodDiary.Core.NotificationAggregate.NotificationPreferences", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FoodDiary.Core.RecipeAggregate.Recipe", b =>
                {
                    b.HasOne("FoodDiary.Core.UserAggregate.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FoodDiary.Core.RecipeAggregate.RecipeIngredient", b =>
                {
                    b.HasOne("FoodDiary.Core.ProductAggregate.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("FoodDiary.Core.RecipeAggregate.Recipe", "Recipe")
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("FoodDiary.Core.RecipeAggregate.Recipe", b =>
                {
                    b.Navigation("Ingredients");
                });

            modelBuilder.Entity("FoodDiary.Core.UserAggregate.User", b =>
                {
                    b.Navigation("FoodEntries");

                    b.Navigation("NotificationPreferences");

                    b.Navigation("Notifications");
                });
#pragma warning restore 612, 618
        }
    }
}
