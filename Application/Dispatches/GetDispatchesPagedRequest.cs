namespace Application.Dispatches;

public class GetDispatchesPagedRequest(string? Cursor, int Limit = 500)
{
    public string? Cursor { get; init; } = Cursor;
    public int Limit { get; init; } = Limit;
}
