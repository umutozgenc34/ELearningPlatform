using ELearningPlatform.Model.Users.Enums;

namespace ELearningPlatform.Model.User.Dtos.Create;


public sealed record CreateUserDto(string UserName,string FirstName,string LastName, string Email, string Password,RoleType Role);
