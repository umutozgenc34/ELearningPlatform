using Microsoft.AspNetCore.Http;

namespace Core.Services;


public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User?.Claims
                .FirstOrDefault(c => c.Type == "user_id")?.Value;

            return userIdClaim ?? string.Empty;
        }
    }

    public string UserName
    {
        get
        {
            var userNameClaim = _httpContextAccessor.HttpContext.User?.Claims
                .FirstOrDefault(c => c.Type == "username")?.Value;

            return userNameClaim ?? string.Empty;
        }
    }
}