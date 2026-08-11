namespace Application.Auth;

public record LoginResponse(string AccessToken, DateTime ExpiresAt);
