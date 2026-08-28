using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Application;
using Infrastructure;
using Microsoft.Extensions.Options;

namespace Presentation;

public static class CloudInfrastructureConfigurationExtensions
{
    public static IServiceCollection AddCloudInfrastructureConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IEventPublisher, SnsEventPublisher>();
        services.AddSingleton<IAmazonSimpleNotificationService>(provider =>
        {
            var serviceUrl = provider.GetRequiredService<IOptions<AppSettings>>().Value.Sns.ServiceUrl;

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
