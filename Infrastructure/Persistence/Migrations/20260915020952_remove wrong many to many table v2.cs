using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removewrongmanytomanytablev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_companies_dispatches_DispatchId",
                table: "companies");

            migrationBuilder.DropIndex(
                name: "IX_companies_DispatchId",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "DispatchId",
                table: "companies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DispatchId",
                table: "companies",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "companies",
                keyColumn: "company_id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "DispatchId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_companies_DispatchId",
                table: "companies",
                column: "DispatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_companies_dispatches_DispatchId",
                table: "companies",
                column: "DispatchId",
                principalTable: "dispatches",
                principalColumn: "dispatch_id");
        }
    }
}
