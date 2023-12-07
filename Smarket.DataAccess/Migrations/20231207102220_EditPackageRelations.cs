using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditPackageRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Inventories_InventoryId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Products_ProductId",
                table: "Packages");

            migrationBuilder.AddColumn<int>(
                name: "InventoryId1",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_InventoryId1",
                table: "Packages",
                column: "InventoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Inventories_InventoryId",
                table: "Packages",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Inventories_InventoryId1",
                table: "Packages",
                column: "InventoryId1",
                principalTable: "Inventories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Products_ProductId",
                table: "Packages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Inventories_InventoryId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Inventories_InventoryId1",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Products_ProductId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_InventoryId1",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "InventoryId1",
                table: "Packages");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Inventories_InventoryId",
                table: "Packages",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Products_ProductId",
                table: "Packages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
