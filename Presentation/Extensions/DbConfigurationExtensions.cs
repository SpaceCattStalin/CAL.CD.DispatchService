using Application.Interfaces;
using Infrastructure;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Presentation;

public static class DbConfigurationExtensions
{
    public static IServiceCollection AddDbConfiguration(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var connectionString = sp.GetRequiredService<IOptions<AppSettings>>().Value.ConnectionStrings.DbConnection;
            options.UseNpgsql(connectionString);
            options.AddInterceptors(new CompanyContextConnectionInterceptor(sp.GetRequiredService<ICurrentUserService>()));
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        return services;
    }
}
