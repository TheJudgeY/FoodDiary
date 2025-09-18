using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace FoodDiary.Infrastructure.Migrations
{
    public partial class AddNutritionalDataToProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CarbohydratesPer100g",
                table: "Products",
                type: "double precision",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FatsPer100g",
                table: "Products",
                type: "double precision",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ProteinsPer100g",
                table: "Products",
                type: "double precision",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CaloriesPer100g", "CarbohydratesPer100g", "Description", "FatsPer100g", "Name", "ProteinsPer100g" },
                values: new object[,]
                {
                    { new Guid("12345678-1234-1234-1234-123456789001"), 165.0, 0.0, "Lean chicken breast, skinless and boneless", 3.6000000000000001, "Chicken Breast", 31.0 },
                    { new Guid("12345678-1234-1234-1234-123456789002"), 111.0, 23.0, "Cooked brown rice", 0.90000000000000002, "Brown Rice", 2.6000000000000001 },
                    { new Guid("12345678-1234-1234-1234-123456789003"), 34.0, 7.0, "Fresh broccoli florets", 0.40000000000000002, "Broccoli", 2.7999999999999998 },
                    { new Guid("12345678-1234-1234-1234-123456789004"), 208.0, 0.0, "Atlantic salmon fillet", 12.0, "Salmon", 25.0 },
                    { new Guid("12345678-1234-1234-1234-123456789005"), 86.0, 20.0, "Baked sweet potato", 0.10000000000000001, "Sweet Potato", 1.6000000000000001 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Name",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("12345678-1234-1234-1234-123456789001"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("12345678-1234-1234-1234-123456789002"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("12345678-1234-1234-1234-123456789003"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("12345678-1234-1234-1234-123456789004"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("12345678-1234-1234-1234-123456789005"));

            migrationBuilder.DropColumn(
                name: "CarbohydratesPer100g",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FatsPer100g",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProteinsPer100g",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
