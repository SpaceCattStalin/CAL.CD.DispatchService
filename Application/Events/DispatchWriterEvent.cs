using Domain;

namespace Application.Events;

public record class DispatchWriterEvent(EventType Type, Guid DispatchId);
