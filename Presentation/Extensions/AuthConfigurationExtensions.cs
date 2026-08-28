using System.Text;
using Application.Auth;
using Application.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Presentation.Services;

namespace Presentation;

public static class AuthConfigurationExtensions
{
    public static IServiceCollection AddAuthenticationAndAuthorizeConfiguration(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<AppSettings>>((options, appSettings) =>
            {
                var jwt = appSettings.Value.Jwt;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
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
