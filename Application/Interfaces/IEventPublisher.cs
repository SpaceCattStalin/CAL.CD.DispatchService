using Application.Events;

namespace Application;

public interface IEventPublisher
{
    public Task Publish(DispatchWriterEvent writerEvent);
    public Task Publish(DispatchDeleteEvent deleteEvent);
    public Task Publish(DispatchUpdateEvent updateEvent);
}
