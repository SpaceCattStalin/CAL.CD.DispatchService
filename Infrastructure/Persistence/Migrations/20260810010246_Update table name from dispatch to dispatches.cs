using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Updatetablenamefromdispatchtodispatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dispatch_drivers_dispatchs_dispatch_id",
                table: "dispatch_drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_dispatchs_stops_dropoff_stop_id",
                table: "dispatchs");

            migrationBuilder.DropForeignKey(
                name: "FK_dispatchs_stops_pickup_stop_id",
                table: "dispatchs");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_dispatchs_dispatch_id",
                table: "vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dispatchs",
                table: "dispatchs");

            migrationBuilder.RenameTable(
                name: "dispatchs",
                newName: "dispatches");

            migrationBuilder.RenameIndex(
                name: "IX_dispatchs_pickup_stop_id",
                table: "dispatches",
                newName: "IX_dispatches_pickup_stop_id");

            migrationBuilder.RenameIndex(
                name: "IX_dispatchs_dropoff_stop_id",
                table: "dispatches",
                newName: "IX_dispatches_dropoff_stop_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dispatches",
                table: "dispatches",
                column: "DispatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_dispatch_drivers_dispatches_dispatch_id",
                table: "dispatch_drivers",
                column: "dispatch_id",
                principalTable: "dispatches",
                principalColumn: "DispatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dispatches_stops_dropoff_stop_id",
                table: "dispatches",
                column: "dropoff_stop_id",
                principalTable: "stops",
                principalColumn: "StopId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dispatches_stops_pickup_stop_id",
                table: "dispatches",
                column: "pickup_stop_id",
                principalTable: "stops",
                principalColumn: "StopId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_dispatches_dispatch_id",
                table: "vehicles",
                column: "dispatch_id",
                principalTable: "dispatches",
                principalColumn: "DispatchId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dispatch_drivers_dispatches_dispatch_id",
                table: "dispatch_drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_dispatches_stops_dropoff_stop_id",
                table: "dispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_dispatches_stops_pickup_stop_id",
                table: "dispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_dispatches_dispatch_id",
                table: "vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dispatches",
                table: "dispatches");

            migrationBuilder.RenameTable(
                name: "dispatches",
                newName: "dispatchs");

            migrationBuilder.RenameIndex(
                name: "IX_dispatches_pickup_stop_id",
                table: "dispatchs",
                newName: "IX_dispatchs_pickup_stop_id");

            migrationBuilder.RenameIndex(
                name: "IX_dispatches_dropoff_stop_id",
                table: "dispatchs",
                newName: "IX_dispatchs_dropoff_stop_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dispatchs",
                table: "dispatchs",
                column: "DispatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_dispatch_drivers_dispatchs_dispatch_id",
                table: "dispatch_drivers",
                column: "dispatch_id",
                principalTable: "dispatchs",
                principalColumn: "DispatchId",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_dispatchs_dispatch_id",
                table: "vehicles",
                column: "dispatch_id",
                principalTable: "dispatchs",
                principalColumn: "DispatchId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
