
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
    [Migration("20250730171914_FixFoodEntryRelationships")]
    partial class FixFoodEntryRelationships
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

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<double>("FatsPer100g")
                        .HasPrecision(8, 2)
                        .HasColumnType("double precision");

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
                            Description = "Baked sweet potato",
                            FatsPer100g = 0.10000000000000001,
                            Name = "Sweet Potato",
                            ProteinsPer100g = 1.6000000000000001
                        });
                });

            modelBuilder.Entity("FoodDiary.Core.ShoppingListAggregate.ShoppingList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("IsCompleted");

                    b.HasIndex("UserId");

                    b.ToTable("ShoppingLists", (string)null);
                });

            modelBuilder.Entity("FoodDiary.Core.ShoppingListAggregate.ShoppingListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsPurchased")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("PurchasedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<Guid>("ShoppingListId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("IsPurchased");

                    b.HasIndex("ProductId");

                    b.HasIndex("ShoppingListId");

                    b.ToTable("ShoppingListItems", (string)null);
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

            modelBuilder.Entity("FoodDiary.Core.ShoppingListAggregate.ShoppingList", b =>
                {
                    b.HasOne("FoodDiary.Core.UserAggregate.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FoodDiary.Core.ShoppingListAggregate.ShoppingListItem", b =>
                {
                    b.HasOne("FoodDiary.Core.ProductAggregate.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FoodDiary.Core.ShoppingListAggregate.ShoppingList", "ShoppingList")
                        .WithMany("Items")
                        .HasForeignKey("ShoppingListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("ShoppingList");
                });

            modelBuilder.Entity("FoodDiary.Core.ShoppingListAggregate.ShoppingList", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("FoodDiary.Core.UserAggregate.User", b =>
                {
                    b.Navigation("FoodEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
