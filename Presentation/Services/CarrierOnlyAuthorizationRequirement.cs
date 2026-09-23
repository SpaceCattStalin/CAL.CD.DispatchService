using System.Security.Claims;
using Application.Auth;
using Domain;
using Microsoft.AspNetCore.Authorization;

namespace Presentation;

public class CarrierOnlyHandler : AuthorizationHandler<CarrierOnlyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CarrierOnlyRequirement requirement)
    {
        var companyTypeClaim = context.User.FindFirstValue(CustomClaimTypes.CompanyType);
        if (!String.IsNullOrEmpty(companyTypeClaim))
            if (companyTypeClaim.Equals("Carrier"))
            {
                context.Succeed(requirement);
            }


        return Task.CompletedTask;
    }
}
public class CarrierOnlyRequirement(CompanyType company_type) : IAuthorizationRequirement
{
    public CompanyType CompanyType { get; } = company_type;
}
