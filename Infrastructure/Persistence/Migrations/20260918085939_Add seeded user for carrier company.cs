using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addseededuserforcarriercompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "company_id", "created_at", "email", "first_name", "is_active", "last_name", "password_hash", "phone", "updated_at", "user_name", "role" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000123"), new Guid("dc63068f-dbfc-422c-8464-f47698fd8905"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "owner@testcarriers.com", "Carrier", true, "Ownerrrrrr", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "1234567890", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "testcarrier", "Owner" },
                    { new Guid("30000000-0000-0000-0000-000000001234"), new Guid("dc63068f-dbfc-422c-8464-f47698fd8905"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "owner@testdriverr.com", "Driver", true, "Driverrrrrr", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "1234567890", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "testdriver", "Driver" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000123"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000001234"));
        }
    }
}
