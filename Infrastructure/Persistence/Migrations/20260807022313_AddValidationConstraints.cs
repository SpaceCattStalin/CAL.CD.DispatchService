using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddValidationConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "instructions",
                table: "dispatchs");

            migrationBuilder.AlterColumn<int>(
                name: "year",
                table: "vehicles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "vin",
                table: "vehicles",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "color",
                table: "vehicles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "users",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "location_name",
                table: "stops",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "contact_phone",
                table: "stops",
                type: "character varying(12)",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "contact_name",
                table: "stops",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "contact_email",
                table: "stops",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "stops",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "carrier_id",
                table: "dispatchs",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "dispatchs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "dropoff_date",
                table: "dispatchs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "pickup_date",
                table: "dispatchs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "company_phone",
                table: "companies",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "company_name",
                table: "companies",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "company_email",
                table: "companies",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "dispatch_drivers",
                columns: table => new
                {
                    dispatch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispatch_drivers", x => new { x.dispatch_id, x.driver_id });
                    table.ForeignKey(
                        name: "FK_dispatch_drivers_dispatchs_dispatch_id",
                        column: x => x.dispatch_id,
                        principalTable: "dispatchs",
                        principalColumn: "DispatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dispatch_drivers_users_driver_id",
                        column: x => x.driver_id,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_vehicles_vin_min_length",
                table: "vehicles",
                sql: "LENGTH(vin) >= 10");

            migrationBuilder.AddCheckConstraint(
                name: "CK_users_email_min_length",
                table: "users",
                sql: "LENGTH(email) >= 15");

            migrationBuilder.AddCheckConstraint(
                name: "CK_users_first_name_min_length",
                table: "users",
                sql: "LENGTH(first_name) >= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_users_last_name_min_length",
                table: "users",
                sql: "LENGTH(last_name) >= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_users_phone_min_length",
                table: "users",
                sql: "LENGTH(phone) >= 10");

            migrationBuilder.AddCheckConstraint(
                name: "CK_users_user_name_min_length",
                table: "users",
                sql: "LENGTH(user_name) >= 6");

            migrationBuilder.AddCheckConstraint(
                name: "CK_stops_address_min_length",
                table: "stops",
                sql: "LENGTH(address) >= 20");

            migrationBuilder.AddCheckConstraint(
                name: "CK_stops_contact_email_min_length",
                table: "stops",
                sql: "LENGTH(contact_email) >= 15");

            migrationBuilder.AddCheckConstraint(
                name: "CK_stops_contact_name_min_length",
                table: "stops",
                sql: "LENGTH(contact_name) >= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_stops_contact_phone_min_length",
                table: "stops",
                sql: "LENGTH(contact_phone) >= 10");

            migrationBuilder.AddCheckConstraint(
                name: "CK_stops_location_name_min_length",
                table: "stops",
                sql: "LENGTH(location_name) >= 10");

            migrationBuilder.AddCheckConstraint(
                name: "CK_dispatchs_dropoff_after_pickup",
                table: "dispatchs",
                sql: "dropoff_date > pickup_date");

            migrationBuilder.AddCheckConstraint(
                name: "CK_companies_company_email_min_length",
                table: "companies",
                sql: "LENGTH(company_email) >= 15");

            migrationBuilder.AddCheckConstraint(
                name: "CK_companies_company_name_min_length",
                table: "companies",
                sql: "LENGTH(company_name) >= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_companies_company_phone_min_length",
                table: "companies",
                sql: "LENGTH(company_phone) >= 10");

            migrationBuilder.CreateIndex(
                name: "IX_dispatch_drivers_driver_id",
                table: "dispatch_drivers",
                column: "driver_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dispatch_drivers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_vehicles_vin_min_length",
                table: "vehicles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_users_email_min_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_users_first_name_min_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_users_last_name_min_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_users_phone_min_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_users_user_name_min_length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_stops_address_min_length",
                table: "stops");

            migrationBuilder.DropCheckConstraint(
                name: "CK_stops_contact_email_min_length",
                table: "stops");

            migrationBuilder.DropCheckConstraint(
                name: "CK_stops_contact_name_min_length",
                table: "stops");

            migrationBuilder.DropCheckConstraint(
                name: "CK_stops_contact_phone_min_length",
                table: "stops");

            migrationBuilder.DropCheckConstraint(
                name: "CK_stops_location_name_min_length",
                table: "stops");

            migrationBuilder.DropCheckConstraint(
                name: "CK_dispatchs_dropoff_after_pickup",
                table: "dispatchs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_companies_company_email_min_length",
                table: "companies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_companies_company_name_min_length",
                table: "companies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_companies_company_phone_min_length",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "description",
                table: "dispatchs");

            migrationBuilder.DropColumn(
                name: "dropoff_date",
                table: "dispatchs");

            migrationBuilder.DropColumn(
                name: "pickup_date",
                table: "dispatchs");

            migrationBuilder.AlterColumn<short>(
                name: "year",
                table: "vehicles",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "vin",
                table: "vehicles",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "color",
                table: "vehicles",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "location_name",
                table: "stops",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "contact_phone",
                table: "stops",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "contact_name",
                table: "stops",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "contact_email",
                table: "stops",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "stops",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "carrier_id",
                table: "dispatchs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "instructions",
                table: "dispatchs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "company_phone",
                table: "companies",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<string>(
                name: "company_name",
                table: "companies",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "company_email",
                table: "companies",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);
        }
    }
}
