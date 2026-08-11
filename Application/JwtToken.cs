namespace Application;

public record JwtToken(string AccessToken, DateTime ExpiresAt);
