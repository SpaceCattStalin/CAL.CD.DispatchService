using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDispatchStops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "dropoff_stop_id",
                table: "dispatchs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "pickup_stop_id",
                table: "dispatchs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_dispatchs_dropoff_stop_id",
                table: "dispatchs",
                column: "dropoff_stop_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispatchs_pickup_stop_id",
                table: "dispatchs",
                column: "pickup_stop_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dispatchs_stops_dropoff_stop_id",
                table: "dispatchs",
                column: "dropoff_stop_id",
                principalTable: "stops",
                principalColumn: "StopId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dispatchs_stops_pickup_stop_id",
                table: "dispatchs",
                column: "pickup_stop_id",
                principalTable: "stops",
                principalColumn: "StopId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dispatchs_stops_dropoff_stop_id",
                table: "dispatchs");

            migrationBuilder.DropForeignKey(
                name: "FK_dispatchs_stops_pickup_stop_id",
                table: "dispatchs");

            migrationBuilder.DropIndex(
                name: "IX_dispatchs_dropoff_stop_id",
                table: "dispatchs");

            migrationBuilder.DropIndex(
                name: "IX_dispatchs_pickup_stop_id",
                table: "dispatchs");

            migrationBuilder.DropColumn(
                name: "dropoff_stop_id",
                table: "dispatchs");

            migrationBuilder.DropColumn(
                name: "pickup_stop_id",
                table: "dispatchs");
        }
    }
}
