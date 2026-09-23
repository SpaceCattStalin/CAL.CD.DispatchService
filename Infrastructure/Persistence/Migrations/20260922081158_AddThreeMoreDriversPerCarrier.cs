using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThreeMoreDriversPerCarrier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "company_id", "created_at", "email", "first_name", "is_active", "last_name", "password_hash", "phone", "updated_at", "user_name", "role" },
                values: new object[,]
                {
                    { new Guid("54000000-0000-0000-0000-000000000001"), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "b.sutton@apexcarriers.com", "Bradley", true, "Sutton", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002016", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bsutton01", "Driver" },
                    { new Guid("54000000-0000-0000-0000-000000000002"), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "r.doyle@bluehorizon.com", "Rachel", true, "Doyle", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002019", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "rdoyle01", "Driver" },
                    { new Guid("54000000-0000-0000-0000-000000000003"), new Guid("50000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "g.lambert@midwestfreight.com", "Gregory", true, "Lambert", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002022", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "glambert01", "Driver" },
                    { new Guid("55000000-0000-0000-0000-000000000001"), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "m.grover@apexcarriers.com", "Melissa", true, "Grover", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002017", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "mgrover01", "Driver" },
                    { new Guid("55000000-0000-0000-0000-000000000002"), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "n.vance@bluehorizon.com", "Nathaniel", true, "Vance", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002020", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nvance01", "Driver" },
                    { new Guid("55000000-0000-0000-0000-000000000003"), new Guid("50000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "v.pruitt@midwestfreight.com", "Vanessa", true, "Pruitt", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002023", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "vpruitt01", "Driver" },
                    { new Guid("56000000-0000-0000-0000-000000000001"), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "w.barton@apexcarriers.com", "Wesley", true, "Barton", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002018", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "wbarton01", "Driver" },
                    { new Guid("56000000-0000-0000-0000-000000000002"), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "p.hayes@bluehorizon.com", "Priscilla", true, "Hayes", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002021", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "phayes01", "Driver" },
                    { new Guid("56000000-0000-0000-0000-000000000003"), new Guid("50000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "h.chambers@midwestfreight.com", "Harold", true, "Chambers", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002024", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "hchambers01", "Driver" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("54000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("54000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("54000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("55000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("55000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("55000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("56000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("56000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("56000000-0000-0000-0000-000000000003"));
        }
    }
}
