namespace Application.Dispatches;

public class GetDispatchBatchRequest(
    IEnumerable<Guid> DispatchIds)
{
    public IEnumerable<Guid> DispatchIds { get; init; } = DispatchIds;
}
