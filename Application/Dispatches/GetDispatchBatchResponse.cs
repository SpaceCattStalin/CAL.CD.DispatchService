namespace Application.Dispatches;

public class GetDispatchBatchResponse(
    IEnumerable<DispatchResponse> Found,
    IEnumerable<Guid> NotFound)
{
    public IEnumerable<DispatchResponse> Found { get; init; } = Found;
    public IEnumerable<Guid> NotFound { get; init; } = NotFound;
}
