using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Inventories_InventoryId1",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_InventoryId1",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "InventoryId1",
                table: "Packages");

            migrationBuilder.AlterColumn<int>(
                name: "InventoryId",
                table: "Packages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InventoryId",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                name: "FK_Packages_Inventories_InventoryId1",
                table: "Packages",
                column: "InventoryId1",
                principalTable: "Inventories",
                principalColumn: "Id");
        }
    }
}
