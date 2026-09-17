using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removewrongmanytomanytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyDispatch");

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
                name: "IX_dispatches_carrier_id",
                table: "dispatches",
                column: "carrier_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispatches_shipper_id",
                table: "dispatches",
                column: "shipper_id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_dispatches_companies_carrier_id",
                table: "dispatches",
                column: "carrier_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dispatches_companies_shipper_id",
                table: "dispatches",
                column: "shipper_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_companies_dispatches_DispatchId",
                table: "companies");

            migrationBuilder.DropForeignKey(
                name: "FK_dispatches_companies_carrier_id",
                table: "dispatches");

            migrationBuilder.DropForeignKey(
                name: "FK_dispatches_companies_shipper_id",
                table: "dispatches");

            migrationBuilder.DropIndex(
                name: "IX_dispatches_carrier_id",
                table: "dispatches");

            migrationBuilder.DropIndex(
                name: "IX_dispatches_shipper_id",
                table: "dispatches");

            migrationBuilder.DropIndex(
                name: "IX_companies_DispatchId",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "DispatchId",
                table: "companies");

            migrationBuilder.CreateTable(
                name: "CompanyDispatch",
                columns: table => new
                {
                    CompaniesCompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    DispatchesDispatchId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDispatch", x => new { x.CompaniesCompanyId, x.DispatchesDispatchId });
                    table.ForeignKey(
                        name: "FK_CompanyDispatch_companies_CompaniesCompanyId",
                        column: x => x.CompaniesCompanyId,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyDispatch_dispatches_DispatchesDispatchId",
                        column: x => x.DispatchesDispatchId,
                        principalTable: "dispatches",
                        principalColumn: "dispatch_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDispatch_DispatchesDispatchId",
                table: "CompanyDispatch",
                column: "DispatchesDispatchId");
        }
    }
}
