namespace Application.Dispatches;

public class PageResponseWithCursor<T>(IEnumerable<T> Items, string? Cursor)
{
    public IEnumerable<T> Items { get; init; } = Items;
    public string? Cursor { get; init; } = Cursor;
}
