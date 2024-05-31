using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class RenameNutritionInfoToNutritionInfos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_NutritionInfo_NutritionInfoId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_NutritionInfoId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NutritionInfo",
                table: "NutritionInfo");

            migrationBuilder.RenameTable(
                name: "NutritionInfo",
                newName: "NutritionInfos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NutritionInfos",
                table: "NutritionInfos",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_NutritionInfos_NutritionInfoId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_NutritionInfoId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NutritionInfos",
                table: "NutritionInfos");

            migrationBuilder.RenameTable(
                name: "NutritionInfos",
                newName: "NutritionInfo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NutritionInfo",
                table: "NutritionInfo",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_NutritionInfoId",
                table: "Products",
                column: "NutritionInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_NutritionInfo_NutritionInfoId",
                table: "Products",
                column: "NutritionInfoId",
                principalTable: "NutritionInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
