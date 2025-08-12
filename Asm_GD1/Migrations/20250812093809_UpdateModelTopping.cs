using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asm_GD1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelTopping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "ProductToppings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "ProductToppings");
        }
    }
}
