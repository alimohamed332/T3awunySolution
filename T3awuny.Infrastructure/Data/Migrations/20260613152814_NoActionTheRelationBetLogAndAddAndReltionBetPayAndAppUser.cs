using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace T3awuny.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NoActionTheRelationBetLogAndAddAndReltionBetPayAndAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_PayerId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Logistics_DeliveryAddressId",
                table: "Logistics");

            migrationBuilder.DropIndex(
                name: "IX_Logistics_PickupAddressId",
                table: "Logistics");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PayerId",
                table: "Payments",
                column: "PayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_DeliveryAddressId",
                table: "Logistics",
                column: "DeliveryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_PickupAddressId",
                table: "Logistics",
                column: "PickupAddressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_PayerId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Logistics_DeliveryAddressId",
                table: "Logistics");

            migrationBuilder.DropIndex(
                name: "IX_Logistics_PickupAddressId",
                table: "Logistics");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PayerId",
                table: "Payments",
                column: "PayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_DeliveryAddressId",
                table: "Logistics",
                column: "DeliveryAddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_PickupAddressId",
                table: "Logistics",
                column: "PickupAddressId",
                unique: true);
        }
    }
}
