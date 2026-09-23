using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addseededcompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "companies",
                columns: new[] { "company_id", "company_email", "company_name", "company_phone", "type", "created_at", "updated_at" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), "contact@apexcarriers.com", "Apex Carriers LLC", "5550001001", "Carrier", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "ops@bluehorizon.com", "Blue Horizon Transport", "5550001002", "Carrier", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "dispatch@midwestfreight.com", "Midwest Freight Solutions", "5550001003", "Carrier", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "shipping@goldenstate.com", "Golden State Manufacturing", "5550001004", "Shipper", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("50000000-0000-0000-0000-000000000005"), "logistics@summitretail.com", "Summit Retail Group", "5550001005", "Shipper", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "company_id", "created_at", "email", "first_name", "is_active", "last_name", "password_hash", "phone", "updated_at", "user_name", "role" },
                values: new object[,]
                {
                    { new Guid("51000000-0000-0000-0000-000000000001"), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "m.alden@apexcarriers.com", "Marcus", true, "Alden", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "malden01", "Owner" },
                    { new Guid("51000000-0000-0000-0000-000000000002"), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "r.whitfield@bluehorizon.com", "Renee", true, "Whitfield", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002004", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "rwhitfield01", "Owner" },
                    { new Guid("51000000-0000-0000-0000-000000000003"), new Guid("50000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "d.foster@midwestfreight.com", "Diana", true, "Foster", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002007", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "dfoster01", "Owner" },
                    { new Guid("51000000-0000-0000-0000-000000000004"), new Guid("50000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "s.grant@goldenstate.com", "Sophia", true, "Grant", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002010", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sgrant01", "Owner" },
                    { new Guid("51000000-0000-0000-0000-000000000005"), new Guid("50000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "v.holloway@summitretail.com", "Victor", true, "Holloway", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002013", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "vholloway01", "Owner" },
                    { new Guid("52000000-0000-0000-0000-000000000001"), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "d.simmons@apexcarriers.com", "Derek", true, "Simmons", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002002", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "dsimmons01", "Driver" },
                    { new Guid("52000000-0000-0000-0000-000000000002"), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "o.bennett@bluehorizon.com", "Oscar", true, "Bennett", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002005", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "obennett01", "Driver" },
                    { new Guid("52000000-0000-0000-0000-000000000003"), new Guid("50000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "t.banks@midwestfreight.com", "Trevor", true, "Banks", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002008", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "tbanks01", "Driver" },
                    { new Guid("52000000-0000-0000-0000-000000000004"), new Guid("50000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "c.reyes@goldenstate.com", "Connor", true, "Reyes", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002011", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "creyes01", "Driver" },
                    { new Guid("52000000-0000-0000-0000-000000000005"), new Guid("50000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "g.pemberton@summitretail.com", "Grace", true, "Pemberton", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002014", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "gpemberton01", "Driver" },
                    { new Guid("53000000-0000-0000-0000-000000000001"), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "n.coleman@apexcarriers.com", "Nathan", true, "Coleman", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002003", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ncoleman01", "Driver" },
                    { new Guid("53000000-0000-0000-0000-000000000002"), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "m.torres@bluehorizon.com", "Miguel", true, "Torres", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002006", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "mtorres01", "Driver" },
                    { new Guid("53000000-0000-0000-0000-000000000003"), new Guid("50000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "i.meyer@midwestfreight.com", "Isaac", true, "Meyer", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002009", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "imeyer01", "Driver" },
                    { new Guid("53000000-0000-0000-0000-000000000004"), new Guid("50000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "j.stanton@goldenstate.com", "Julia", true, "Stanton", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002012", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "jstanton01", "Driver" },
                    { new Guid("53000000-0000-0000-0000-000000000005"), new Guid("50000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "e.ramirez@summitretail.com", "Ethan", true, "Ramirez", "AQAAAAIAAYagAAAAEBLXzaXNLvzcwr7crtuiu+QvBo1L4LRPzYYijwQASmFIWKWw1/zyh8MKGjf+gyF1jg==", "5550002015", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "eramirez01", "Driver" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("51000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("51000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("51000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("51000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("51000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("52000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("52000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("52000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("52000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("52000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("53000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("53000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("53000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("53000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: new Guid("53000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "company_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "company_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "company_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "company_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "company_id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"));
        }
    }
}
