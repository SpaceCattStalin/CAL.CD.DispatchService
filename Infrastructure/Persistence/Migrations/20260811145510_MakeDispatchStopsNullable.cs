using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeDispatchStopsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dispatches_stops_dropoff_stop_id",
                table: "dispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_dispatches_stops_pickup_stop_id",
                table: "dispatches");

            migrationBuilder.AlterColumn<Guid>(
                name: "pickup_stop_id",
                table: "dispatches",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "dropoff_stop_id",
                table: "dispatches",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_dispatches_stops_dropoff_stop_id",
                table: "dispatches",
                column: "dropoff_stop_id",
                principalTable: "stops",
                principalColumn: "stop_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_dispatches_stops_pickup_stop_id",
                table: "dispatches",
                column: "pickup_stop_id",
                principalTable: "stops",
                principalColumn: "stop_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dispatches_stops_dropoff_stop_id",
                table: "dispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_dispatches_stops_pickup_stop_id",
                table: "dispatches");

            migrationBuilder.AlterColumn<Guid>(
                name: "pickup_stop_id",
                table: "dispatches",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "dropoff_stop_id",
                table: "dispatches",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_dispatches_stops_dropoff_stop_id",
                table: "dispatches",
                column: "dropoff_stop_id",
                principalTable: "stops",
                principalColumn: "stop_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dispatches_stops_pickup_stop_id",
                table: "dispatches",
                column: "pickup_stop_id",
                principalTable: "stops",
                principalColumn: "stop_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
