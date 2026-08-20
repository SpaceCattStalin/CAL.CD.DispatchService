using Domain;

namespace Application.Events;

public record class DispatchDeleteEvent(EventType Type, Guid DispatchId);
