namespace Application.Dispatches;

public record PageResponseWithCursor<T>(IEnumerable<T> Items, string? Cursor);
