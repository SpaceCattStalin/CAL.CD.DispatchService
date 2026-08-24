namespace Application.Dispatches;

public record GetDispatchesPagedRequest(string? Cursor, int Limit = 500);
