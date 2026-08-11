using Application;
using Domain;
using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity;

namespace Infrastructure;

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required", nameof(password));

        return _hasher.HashPassword(user: null!, password);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(user: null!, hashedPassword, providedPassword);
        return result != PasswordVerificationResult.Failed;
    }
}
