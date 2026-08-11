namespace Application.Dispatches;

public record GetDispatchBatchResponse(
    IEnumerable<DispatchResponse> Found,
    IEnumerable<Guid> NotFound);
