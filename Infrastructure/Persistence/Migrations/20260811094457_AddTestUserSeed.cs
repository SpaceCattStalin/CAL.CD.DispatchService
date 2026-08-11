using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTestUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "companies",
                columns: new[] { "company_id", "company_email", "company_name", "company_phone", "type", "created_at", "updated_at" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000001"), "contact@testlogistics.com", "Test Logistics Co", "1234567890", "Shipper", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "company_id", "created_at", "email", "first_name", "is_active", "last_name", "password_hash", "phone", "updated_at", "user_name", "role" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000002"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "owner@testlogistics.com", "Tester", true, "Owner", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "1234567890", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "testowner", "Owner" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "company_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"));
        }
    }
}
