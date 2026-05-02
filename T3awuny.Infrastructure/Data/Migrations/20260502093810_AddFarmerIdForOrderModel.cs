using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace T3awuny.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFarmerIdForOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FarmerId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FarmerId",
                table: "Orders");
        }
    }
}
