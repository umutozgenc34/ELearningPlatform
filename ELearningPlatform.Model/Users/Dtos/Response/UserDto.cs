namespace ELearningPlatform.Model.User.Dtos.Response;

public sealed record UserDto(string Id, string UserName, string Email)
{
    public List<string> Roles { get; set; } = new List<string>();
}

