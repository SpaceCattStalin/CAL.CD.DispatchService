using System.Data.Common;
using Application.Auth;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

/// <summary>
/// Sets the Postgres session GUC "app.company_id" on every connection open, so
/// row-level security policies (e.g. on "dispatches") can read the acting
/// company via current_setting('app.company_id', true). Must run on every
/// open (not just the first) because Npgsql pools physical connections and
/// does not reset session-level GUC state on return to the pool.
/// </summary>
public class CompanyContextConnectionInterceptor(ICurrentUserService currentUserService) : DbConnectionInterceptor
{
    private const string SetCompanyIdSql = "SELECT set_config('app.company_id', @company_id, false);";

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        using var command = CreateCommand(connection);
        command.ExecuteNonQuery();

        base.ConnectionOpened(connection, eventData);
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await using var command = CreateCommand(connection);
        await command.ExecuteNonQueryAsync(cancellationToken);

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    private DbCommand CreateCommand(DbConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = SetCompanyIdSql;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "company_id";
        parameter.Value = TryGetCompanyId(out var companyId) ? companyId.ToString() : DBNull.Value;
        command.Parameters.Add(parameter);

        var bypassParam = command.CreateParameter();
        bypassParam.ParameterName = "is_sync_job";
        bypassParam.Value = currentUserService.HasPermission(PermissionNames.DispatchesReadAll) ? "true" : "false";
        command.Parameters.Add(bypassParam);

        return command;
    }

    private bool TryGetCompanyId(out Guid companyId)
    {
        try
        {
            companyId = currentUserService.CompanyId;
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            companyId = Guid.Empty;
            return false;
        }
    }
}
