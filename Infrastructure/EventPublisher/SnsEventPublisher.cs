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

        string message = JsonSerializer.Serialize(writerEvent);

        await sns.PublishAsync(topicArn, message);
    }

    public async Task Publish(DispatchDeleteEvent deleteEvent)
    {
        var topicArn = configuration["Sns:TopicArn"]
            ?? throw new InvalidOperationException("Sns:TopicArn was not found.");

        string message = JsonSerializer.Serialize(deleteEvent);

        await sns.PublishAsync(topicArn, message);
    }

    public async Task Publish(DispatchUpdateEvent updateEvent)
    {
        var topicArn = configuration["Sns:TopicArn"]
               ?? throw new InvalidOperationException("Sns:TopicArn was not found.");

        string messages = JsonSerializer.Serialize(updateEvent);

        await sns.PublishAsync(topicArn, messages);
    }

}
