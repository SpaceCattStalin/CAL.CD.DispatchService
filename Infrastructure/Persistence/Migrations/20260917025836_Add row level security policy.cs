using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addrowlevelsecuritypolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE dispatches ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE dispatches FORCE ROW LEVEL SECURITY;");

            migrationBuilder.Sql(@"
                CREATE POLICY dispatch_tenant_isolation ON dispatches
                USING(
                    shipper_id = NULLIF(current_setting('app.company_id', true), '')::uuid
                    OR carrier_id = NULLIF(current_setting('app.company_id', true), '')::uuid
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY dispatch_tenant_isolation ON dispatches;");
            migrationBuilder.Sql("ALTER TABLE dispatches DISABLE ROW LEVEL SECURITY;");
        }
    }
}
