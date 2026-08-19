using Application;
using Application.Events;
using Amazon.SimpleNotificationService;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class SnsEventPublisher(IAmazonSimpleNotificationService sns, IConfiguration configuration) : IEventPublisher
{

    public async Task Publish(DispatchWriterEvent writerEvent)
    {
        var topicArn = configuration["Sns:TopicArn"]
            ?? throw new InvalidOperationException("Sns:TopicArn was not found.");

        string message = JsonSerializer.Serialize<DispatchWriterEvent>(writerEvent);

        await sns.PublishAsync(topicArn, message);
    }
}
