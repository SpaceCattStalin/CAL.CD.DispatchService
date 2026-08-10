using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamePrimaryKeyColumnsToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "vehicles",
                newName: "vehicle_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "StopId",
                table: "stops",
                newName: "stop_id");

            migrationBuilder.RenameColumn(
                name: "DispatchId",
                table: "dispatches",
                newName: "dispatch_id");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "companies",
                newName: "company_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "vehicle_id",
                table: "vehicles",
                newName: "VehicleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "stop_id",
                table: "stops",
                newName: "StopId");

            migrationBuilder.RenameColumn(
                name: "dispatch_id",
                table: "dispatches",
                newName: "DispatchId");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "companies",
                newName: "CompanyId");
        }
    }
}
