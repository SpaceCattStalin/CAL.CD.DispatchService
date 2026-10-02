using System.Data.Common;
using Application.Auth;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

/// <summary>
/// Set the row level security for Postgres to constraint fetching only 
/// dispatches that belong to the user shipper company and the carrier company involve
/// </summary>
public class CompanyContextConnectionInterceptor(ICurrentUserService currentUserService) : DbConnectionInterceptor
{
    private const string SetCompanyIdSql = """
        SELECT set_config('app.company_id', @company_id, false), 
        set_config('app.is_sync_job', @is_sync_job, false);
        """;

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
