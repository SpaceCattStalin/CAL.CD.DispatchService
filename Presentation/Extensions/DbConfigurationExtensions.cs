using Application.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Presentation;

public static class DbConfigurationExtensions
{
    public static IServiceCollection AddDbConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection") ?? throw new InvalidOperationException("Connection string was not found."); ;
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        return services;
    }
}
