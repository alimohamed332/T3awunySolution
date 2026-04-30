using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace T3awuny.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitPropertyToOrderItemModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemOrdered_Unit",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemOrdered_Unit",
                table: "OrderItems");
        }
    }
}
