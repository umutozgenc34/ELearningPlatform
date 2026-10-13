namespace Core.Services;

public class IdentityServiceFake : IIdentityService
{
    public string GetUserId => "fakeuserid";
    public string UserName => "umutozgenc34";
    public bool IsInRole(string role) => false;
}
