using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDiary.Infrastructure.Migrations
{
    public partial class AddFoodEntries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodEntryProducts");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "FoodEntries",
                newName: "ConsumedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "FoodEntries",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealType",
                table: "FoodEntries",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "FoodEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "FoodEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WeightGrams",
                table: "FoodEntries",
                type: "double precision",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_FoodEntries_ConsumedAt",
                table: "FoodEntries",
                column: "ConsumedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FoodEntries_ProductId",
                table: "FoodEntries",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodEntries_UserId_ConsumedAt",
                table: "FoodEntries",
                columns: new[] { "UserId", "ConsumedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FoodEntries_UserId1",
                table: "FoodEntries",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodEntries_Products_ProductId",
                table: "FoodEntries",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodEntries_Users_UserId1",
                table: "FoodEntries",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodEntries_Products_ProductId",
                table: "FoodEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodEntries_Users_UserId1",
                table: "FoodEntries");

            migrationBuilder.DropIndex(
                name: "IX_FoodEntries_ConsumedAt",
                table: "FoodEntries");

            migrationBuilder.DropIndex(
                name: "IX_FoodEntries_ProductId",
                table: "FoodEntries");

            migrationBuilder.DropIndex(
                name: "IX_FoodEntries_UserId_ConsumedAt",
                table: "FoodEntries");

            migrationBuilder.DropIndex(
                name: "IX_FoodEntries_UserId1",
                table: "FoodEntries");

            migrationBuilder.DropColumn(
                name: "MealType",
                table: "FoodEntries");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "FoodEntries");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "FoodEntries");

            migrationBuilder.DropColumn(
                name: "WeightGrams",
                table: "FoodEntries");

            migrationBuilder.RenameColumn(
                name: "ConsumedAt",
                table: "FoodEntries",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "FoodEntries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FoodEntryProducts",
                columns: table => new
                {
                    FoodEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityGrams = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodEntryProducts", x => new { x.FoodEntryId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_FoodEntryProducts_FoodEntries_FoodEntryId",
                        column: x => x.FoodEntryId,
                        principalTable: "FoodEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodEntryProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodEntryProducts_ProductId",
                table: "FoodEntryProducts",
                column: "ProductId");
        }
    }
}
