using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class RenameIngredientToIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductIngredients_Ingredient_IngredientsId",
                table: "ProductIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_NutritionInfo_NutritionInfoId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ingredient",
                table: "Ingredient");

            migrationBuilder.RenameTable(
                name: "Ingredient",
                newName: "Ingredients");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredient_Name",
                table: "Ingredients",
                newName: "IX_Ingredients_Name");

            migrationBuilder.AlterColumn<int>(
                name: "NutritionInfoId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ingredients",
                table: "Ingredients",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIngredients_Ingredients_IngredientsId",
                table: "ProductIngredients",
                column: "IngredientsId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_NutritionInfo_NutritionInfoId",
                table: "Products",
                column: "NutritionInfoId",
                principalTable: "NutritionInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductIngredients_Ingredients_IngredientsId",
                table: "ProductIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_NutritionInfo_NutritionInfoId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ingredients",
                table: "Ingredients");

            migrationBuilder.RenameTable(
                name: "Ingredients",
                newName: "Ingredient");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_Name",
                table: "Ingredient",
                newName: "IX_Ingredient_Name");

            migrationBuilder.AlterColumn<int>(
                name: "NutritionInfoId",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ingredient",
                table: "Ingredient",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIngredients_Ingredient_IngredientsId",
                table: "ProductIngredients",
                column: "IngredientsId",
                principalTable: "Ingredient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_NutritionInfo_NutritionInfoId",
                table: "Products",
                column: "NutritionInfoId",
                principalTable: "NutritionInfo",
                principalColumn: "Id");
        }
    }
}
