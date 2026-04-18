using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace T3awuny.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddFromFarmerAndAddAppUserObjectToFarmerAndTrader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmerProfiles_Addresses_FarmAddressId",
                table: "FarmerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_FarmerProfiles_FarmAddressId",
                table: "FarmerProfiles");

            migrationBuilder.DropColumn(
                name: "FarmAddressId",
                table: "FarmerProfiles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VerifiedAt",
                table: "FarmerProfiles",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "VerifiedAt",
                table: "FarmerProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FarmAddressId",
                table: "FarmerProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FarmerProfiles_FarmAddressId",
                table: "FarmerProfiles",
                column: "FarmAddressId",
                unique: true,
                filter: "[FarmAddressId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmerProfiles_Addresses_FarmAddressId",
                table: "FarmerProfiles",
                column: "FarmAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
