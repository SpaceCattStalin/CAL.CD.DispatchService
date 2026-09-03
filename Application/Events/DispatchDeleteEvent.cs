using Domain;

namespace Application.Events;

public class DispatchDeleteEvent(EventType Type, Guid DispatchId)
{
    public EventType Type { get; } = Type;
    public Guid DispatchId { get; } = DispatchId;
}
