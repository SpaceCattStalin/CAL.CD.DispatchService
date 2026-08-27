namespace Application.Auth;

public class LoginRequest(string UserName, string Password)
{
    public string UserName { get; init; } = UserName;
    public string Password { get; init; } = Password;
}
