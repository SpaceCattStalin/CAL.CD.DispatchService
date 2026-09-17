using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addcompanydispatchrealtionship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyDispatch");
        }
    }
}
