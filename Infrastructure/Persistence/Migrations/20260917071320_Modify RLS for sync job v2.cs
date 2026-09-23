using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyRLSforsyncjobv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER POLICY dispatch_tenant_isolation ON dispatches
                USING (
                    shipper_id = NULLIF(current_setting('app.company_id', true), '')::uuid
                    OR carrier_id = NULLIF(current_setting('app.company_id', true), '')::uuid
                    OR NULLIF(current_setting('app.is_sync_job', true), '') = 'true'
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER POLICY dispatch_tenant_isolation ON dispatches
                USING (
                    shipper_id = NULLIF(current_setting('app.company_id', true), '')::uuid
                    OR carrier_id = NULLIF(current_setting('app.company_id', true), '')::uuid
                    OR NULLIF(current_setting('app.bypass_tenant_filter', true), '') = 'true'
                );
                """);
        }
    }
}
