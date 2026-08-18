using System.Text;
using Application.Auth;
using Application.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Presentation.Services;

namespace Presentation;

public static class AuthConfigurationExtensions
{
    public static IServiceCollection AddAuthenticationAndAuthorizeConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt settings were not found.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
                    ValidateLifetime = true
                };
            });

        // Must register an instance (not the type) here: PermissionAuthorizationRequirement's constructor
        // takes params string[] allowedPermissions, which DI can't resolve on its own.
        services.AddSingleton<IAuthorizationHandler>(new PermissionAuthorizationRequirement());

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            foreach (var permission in PermissionNames.All)
            {
                options.AddPolicy(permission, policy =>
                    policy.Requirements.Add(new PermissionAuthorizationRequirement(permission)));
            }
        });

        return services;
    }
}
