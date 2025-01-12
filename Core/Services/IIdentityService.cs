namespace Core.Services;

public interface IIdentityService
{
    public string GetUserId { get; }
    public string UserName { get; }
}