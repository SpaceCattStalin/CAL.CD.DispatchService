using Application.Events;

namespace Application;

public interface IEventPublisher
{
    public Task Publish(DispatchWriterEvent writerEvent);
}
