using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Core.Services;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string GetUserId =>
        User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User?.FindFirst("nameid")?.Value
        ?? User?.FindFirst("sub")?.Value
        ?? string.Empty;

    public string UserName =>
        User?.FindFirst(ClaimTypes.Name)?.Value
        ?? User?.FindFirst("unique_name")?.Value
        ?? string.Empty;

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
