namespace Application.Auth;

public class JwtToken(string AccessToken, DateTime ExpiresAt)
{
    public string AccessToken { get; init; } = AccessToken;
    public DateTime ExpiresAt { get; init; } = ExpiresAt;
}
