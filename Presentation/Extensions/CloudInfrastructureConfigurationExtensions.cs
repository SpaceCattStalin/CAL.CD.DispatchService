using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Application;
using Infrastructure;

namespace Presentation;

public static class CloudInfrastructureConfigurationExtensions
{
    public static IServiceCollection AddCloudInfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventPublisher, SnsEventPublisher>();
        services.AddSingleton<IAmazonSimpleNotificationService>(provider =>
        {
            var serviceUrl = configuration["Sns:ServiceUrl"]
                ?? throw new InvalidOperationException("Sns:ServiceUrl was not found.");

            var snsConfig = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = serviceUrl
            };
            var credential = new BasicAWSCredentials("", "");
            return new AmazonSimpleNotificationServiceClient(credential, snsConfig);
        });

        return services;
    }
}
