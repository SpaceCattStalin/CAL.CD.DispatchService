using Application;
using Application.Events;
using Amazon.SimpleNotificationService;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Amazon.SimpleNotificationService.Model;

namespace Infrastructure;

public class SnsEventPublisher(IAmazonSimpleNotificationService sns, IOptions<AppSettings> appSettings) : IEventPublisher
{
    private readonly string _topicArn = appSettings.Value.Sns.TopicArn;

    public async Task Publish(DispatchWriterEvent writerEvent)
    {
        try
        {
            string message = JsonSerializer.Serialize(writerEvent);

            await sns.PublishAsync(_topicArn, message);
        }
        catch (NotFoundException exception)
        {
            Console.WriteLine("=============Error code: {0}, with error detail: {1}============", exception.ErrorCode, exception.Message);
        }
    }

    public async Task Publish(DispatchDeleteEvent deleteEvent)
    {
        string message = JsonSerializer.Serialize(deleteEvent);

        await sns.PublishAsync(_topicArn, message);
    }

    public async Task Publish(DispatchUpdateEvent updateEvent)
    {
        string messages = JsonSerializer.Serialize(updateEvent);

        await sns.PublishAsync(_topicArn, messages);
    }

}
