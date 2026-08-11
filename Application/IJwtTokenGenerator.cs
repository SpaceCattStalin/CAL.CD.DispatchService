namespace Application;

public interface IJwtTokenGenerator
{
    JwtToken GenerateToken(Guid userId, string userName, string roleName, IEnumerable<string> permissions);
}
