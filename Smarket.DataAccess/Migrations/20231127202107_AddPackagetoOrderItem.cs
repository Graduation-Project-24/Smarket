using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPackagetoOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Packages_PackageId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "PackageId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Packages_PackageId",
                table: "OrderItems",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Packages_PackageId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "PackageId",
                table: "OrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Packages_PackageId",
                table: "OrderItems",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");
        }
    }
}
