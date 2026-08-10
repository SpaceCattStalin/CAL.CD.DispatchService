using Application;

namespace Presentation.Services;

public class CurrentUserService : ICurrentUserService
{
    // TODO: replace with real claim extraction (httpContext.HttpContext.User) once authentication is implemented
    public Guid ShipperId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public CurrentUserService(IHttpContextAccessor httpContext)
    {
    }
}
