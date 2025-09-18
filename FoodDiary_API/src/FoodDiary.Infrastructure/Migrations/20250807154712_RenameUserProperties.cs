using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDiary.Infrastructure.Migrations
{
    public partial class RenameUserProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserGender",
                table: "Users",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "UserActivityLevel",
                table: "Users",
                newName: "ActivityLevel");

            migrationBuilder.RenameColumn(
                name: "UserFitnessGoal",
                table: "Users",
                newName: "FitnessGoal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Users",
                newName: "UserGender");

            migrationBuilder.RenameColumn(
                name: "ActivityLevel",
                table: "Users",
                newName: "UserActivityLevel");

            migrationBuilder.RenameColumn(
                name: "FitnessGoal",
                table: "Users",
                newName: "UserFitnessGoal");
        }
    }
}
