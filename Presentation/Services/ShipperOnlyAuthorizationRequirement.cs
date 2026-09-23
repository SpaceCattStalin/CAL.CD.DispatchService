using System.Security.Claims;
using Application.Auth;
using Domain;
using Microsoft.AspNetCore.Authorization;

namespace Presentation;

public class ShipperOnlyHandler : AuthorizationHandler<ShipperOnlyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ShipperOnlyRequirement requirement)
    {
        Console.WriteLine(context.User.FindFirstValue(CustomClaimTypes.CompanyType));

        var companyTypeClaim = context.User.FindFirstValue(CustomClaimTypes.CompanyType);
        if (!String.IsNullOrEmpty(companyTypeClaim))
            if (companyTypeClaim.Equals("Shipper"))
            {
                context.Succeed(requirement);
            }


        return Task.CompletedTask;
    }
}


public class ShipperOnlyRequirement(CompanyType company_type) : IAuthorizationRequirement
{
    public CompanyType CompanyType { get; } = company_type;
}
