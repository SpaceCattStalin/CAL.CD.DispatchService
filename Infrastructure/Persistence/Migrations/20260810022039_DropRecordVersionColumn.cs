using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropRecordVersionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE stops ALTER COLUMN stop_number TYPE integer USING stop_number::integer;");

            migrationBuilder.DropColumn(
                name: "record_version",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "record_version",
                table: "users");

            migrationBuilder.DropColumn(
                name: "record_version",
                table: "stops");

            migrationBuilder.DropColumn(
                name: "record_version",
                table: "dispatches");

            migrationBuilder.DropColumn(
                name: "record_version",
                table: "companies");

            migrationBuilder.AlterColumn<int>(
                name: "stop_number",
                table: "stops",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "dispatch_drivers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "dispatch_drivers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "dispatch_drivers");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "dispatch_drivers");

            migrationBuilder.AlterColumn<string>(
                name: "stop_number",
                table: "stops",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<byte[]>(
                name: "record_version",
                table: "vehicles",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[] { });

            migrationBuilder.AddColumn<byte[]>(
                name: "record_version",
                table: "users",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[] { });

            migrationBuilder.AddColumn<byte[]>(
                name: "record_version",
                table: "stops",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[] { });

            migrationBuilder.AddColumn<byte[]>(
                name: "record_version",
                table: "dispatches",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[] { });

            migrationBuilder.AddColumn<byte[]>(
                name: "record_version",
                table: "companies",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[] { });
        }
    }
}
