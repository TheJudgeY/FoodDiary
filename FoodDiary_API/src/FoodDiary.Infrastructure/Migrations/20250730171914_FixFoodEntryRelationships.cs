using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDiary.Infrastructure.Migrations
{
    public partial class FixFoodEntryRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodEntries_Users_UserId1",
                table: "FoodEntries");

            migrationBuilder.DropIndex(
                name: "IX_FoodEntries_UserId1",
                table: "FoodEntries");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "FoodEntries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "FoodEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodEntries_UserId1",
                table: "FoodEntries",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodEntries_Users_UserId1",
                table: "FoodEntries",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
