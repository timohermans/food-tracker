using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class ProductNutritionInfoOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_NutritionInfos_NutritionInfoId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_NutritionInfoId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "NutritionInfoId",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Products_NutritionInfoId",
                table: "Products",
                column: "NutritionInfoId",
                unique: true,
                filter: "[NutritionInfoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_NutritionInfos_NutritionInfoId",
                table: "Products",
                column: "NutritionInfoId",
                principalTable: "NutritionInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_NutritionInfos_NutritionInfoId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_NutritionInfoId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "NutritionInfoId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_NutritionInfoId",
                table: "Products",
                column: "NutritionInfoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_NutritionInfos_NutritionInfoId",
                table: "Products",
                column: "NutritionInfoId",
                principalTable: "NutritionInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
