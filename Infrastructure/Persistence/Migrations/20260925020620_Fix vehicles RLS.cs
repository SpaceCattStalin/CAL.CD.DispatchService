using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixvehiclesRLS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE vehicles ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE vehicles FORCE ROW LEVEL SECURITY;");

            migrationBuilder.Sql(@"
            CREATE POLICY vehicle_tenant_isolation ON vehicles
            USING (
                dispatch_id IN (
                    SELECT dispatch_id FROM dispatches
                    WHERE shipper_id = NULLIF(current_setting('app.company_id', true), '')::uuid
                    OR carrier_id = NULLIF(current_setting('app.company_id', true), '')::uuid
                )
            );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY vehicle_tenant_isolation ON vehicles");
            migrationBuilder.Sql("ALTER TABLE vehicles DISABLE ROW LEVEL SECURITY;");
        }
    }
}
