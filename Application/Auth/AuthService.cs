using Application;
using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth;

public class AuthService
{
    private readonly IApplicationDbContext _db;
    private readonly IValidator<LoginRequest> _validator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IApplicationDbContext db, IValidator<LoginRequest> validator,
        IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _db = db;
        _validator = validator;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var user = await _db.Users.SingleOrDefaultAsync(u => u.UserName == request.UserName);
        if (user is null || !user.IsActive || !_passwordHasher.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var roleName = user.UserRole.ToString();
        var role = await _db.Roles
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .SingleOrDefaultAsync(r => r.Name == roleName);

        var permissions = role.RolePermissions.Select(rp => rp.Permission.Name);
        var token = _tokenGenerator.GenerateToken(user.UserId, user.UserName, roleName, permissions);

        return new LoginResponse(token.AccessToken, token.ExpiresAt);
    }
}
