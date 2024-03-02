using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAxiesToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "XAxies",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YAxies",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XAxies",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YAxies",
                table: "Products");
        }
    }
}
