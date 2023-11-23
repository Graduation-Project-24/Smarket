using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixingModelsToNewErd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_CartItems_CartItemId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_OrderItems_OrderItemId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_CartItemId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_OrderItemId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "OrderItemId",
                table: "Packages",
                newName: "left");

            migrationBuilder.AddColumn<int>(
                name: "BrandId1",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId1",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "OrderItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId1",
                table: "Products",
                column: "BrandId1");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SubCategoryId1",
                table: "Products",
                column: "SubCategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_PackageId",
                table: "OrderItems",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_PackageId",
                table: "CartItems",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Packages_PackageId",
                table: "CartItems",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Packages_PackageId",
                table: "OrderItems",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId1",
                table: "Products",
                column: "BrandId1",
                principalTable: "Brands",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SubCategories_SubCategoryId1",
                table: "Products",
                column: "SubCategoryId1",
                principalTable: "SubCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Packages_PackageId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Packages_PackageId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brands_BrandId1",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_SubCategories_SubCategoryId1",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_BrandId1",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SubCategoryId1",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_PackageId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_PackageId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "BrandId1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubCategoryId1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "left",
                table: "Packages",
                newName: "OrderItemId");

            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CartItemId",
                table: "Packages",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_OrderItemId",
                table: "Packages",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_CartItems_CartItemId",
                table: "Packages",
                column: "CartItemId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_OrderItems_OrderItemId",
                table: "Packages",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
