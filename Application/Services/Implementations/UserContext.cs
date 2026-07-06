using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ProjectPlanner.Application.Services.Implementations;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }

    public int UserId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("No HTTP context available.");

            var value = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new InvalidOperationException("User is not authenticated or missing NameIdentifier claim.");

            return int.Parse(value);
        }
    }
}
