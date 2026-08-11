namespace Application.Dispatches;

public record GetDispatchBatchRequest(
    IEnumerable<Guid> DispatchIds);
