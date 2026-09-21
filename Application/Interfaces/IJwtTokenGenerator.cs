using Application.Auth;

namespace Application.Interfaces;

public interface IJwtTokenGenerator
{
    JwtToken GenerateToken(Guid userId, string userName, string roleName, string lastName, string firstName, Guid companyId, string companyName, string companyType, IEnumerable<string> permissions);
}
