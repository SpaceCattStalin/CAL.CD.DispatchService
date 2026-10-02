using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameSyncReadAllAndAddSyncUpdateAllPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "permission_id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000b"),
                column: "name",
                value: "sync:read-all");

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "permission_id", "created_at", "name", "updated_at" },
                values: new object[] { new Guid("20000000-0000-0000-0000-00000000000d"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sync:update-all", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id", "created_at", "updated_at" },
                values: new object[] { new Guid("20000000-0000-0000-0000-00000000000d"), new Guid("90000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-00000000000d"), new Guid("90000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "permission_id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000d"));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "permission_id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000b"),
                column: "name",
                value: "dispatches:read-all");
        }
    }
}
