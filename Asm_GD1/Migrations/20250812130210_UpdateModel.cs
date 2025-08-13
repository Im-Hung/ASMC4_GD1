using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asm_GD1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "ProductToppings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "ProductToppings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
