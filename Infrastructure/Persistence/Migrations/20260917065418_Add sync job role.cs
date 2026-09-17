using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addsyncjobrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "permission_id", "created_at", "name", "updated_at" },
                values: new object[] { new Guid("20000000-0000-0000-0000-00000000000b"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "dispatches:read-all", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "role_id", "created_at", "name", "updated_at" },
                values: new object[] { new Guid("90000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SyncJob", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "company_id", "created_at", "email", "first_name", "is_active", "last_name", "password_hash", "phone", "updated_at", "user_name", "role" },
                values: new object[] { new Guid("90000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "syncjob@placeholder.com", "SyncJobbbb", true, "SyncJobbb", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "12345678910", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "SyncJobbbb", "SyncJob" });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id", "created_at", "updated_at" },
                values: new object[] { new Guid("20000000-0000-0000-0000-00000000000b"), new Guid("90000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-00000000000b"), new Guid("90000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "permission_id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000b"));

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "role_id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"));
        }
    }
}
