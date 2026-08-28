using System.ComponentModel.DataAnnotations;

namespace Infrastructure;

public class AppSettings
{
    [Required]
    public required ConnectionStringsSettings ConnectionStrings { get; init; }

    [Required]
    public required JwtSettings Jwt { get; init; }

    [Required]
    public required SnsSettings Sns { get; init; }
}

public class ConnectionStringsSettings
{
    [Required]
    public required string DbConnection { get; init; }
}

public class JwtSettings
{
    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Required]
    public required string SigningKey { get; init; }

    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; init; }
}

public class SnsSettings
{
    [Required]
    public required string ServiceUrl { get; init; }

    [Required]
    public required string TopicArn { get; init; }
}
