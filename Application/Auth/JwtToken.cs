namespace Application.Auth;

public record JwtToken(string AccessToken, DateTime ExpiresAt);
