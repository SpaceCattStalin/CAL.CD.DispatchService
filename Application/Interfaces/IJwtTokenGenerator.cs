using Application.Auth;

namespace Application.Interfaces;

public interface IJwtTokenGenerator
{
    JwtToken GenerateToken(Guid userId, string userName, string roleName, Guid companyId, IEnumerable<string> permissions);
}
