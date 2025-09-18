
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
    [Migration("20250728134710_InitialCreate")]
    partial class InitialCreate
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

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("FoodEntries");
                });

            modelBuilder.Entity("FoodDiary.Core.FoodEntryAggregate.FoodEntryProduct", b =>
                {
                    b.Property<Guid>("FoodEntryId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<double>("QuantityGrams")
                        .HasColumnType("double precision");

                    b.HasKey("FoodEntryId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("FoodEntryProducts");
                });

            modelBuilder.Entity("FoodDiary.Core.ProductAggregate.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("CaloriesPer100g")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("FoodDiary.Core.UserAggregate.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FoodDiary.Core.FoodEntryAggregate.FoodEntry", b =>
                {
                    b.HasOne("FoodDiary.Core.UserAggregate.User", "User")
                        .WithMany("FoodEntries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FoodDiary.Core.FoodEntryAggregate.FoodEntryProduct", b =>
                {
                    b.HasOne("FoodDiary.Core.FoodEntryAggregate.FoodEntry", "FoodEntry")
                        .WithMany("FoodEntryProducts")
                        .HasForeignKey("FoodEntryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FoodDiary.Core.ProductAggregate.Product", "Product")
                        .WithMany("FoodEntryProducts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FoodEntry");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("FoodDiary.Core.FoodEntryAggregate.FoodEntry", b =>
                {
                    b.Navigation("FoodEntryProducts");
                });

            modelBuilder.Entity("FoodDiary.Core.ProductAggregate.Product", b =>
                {
                    b.Navigation("FoodEntryProducts");
                });

            modelBuilder.Entity("FoodDiary.Core.UserAggregate.User", b =>
                {
                    b.Navigation("FoodEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
