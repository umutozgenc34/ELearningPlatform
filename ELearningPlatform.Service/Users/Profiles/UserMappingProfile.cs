using AutoMapper;
using ELearningPlatform.Model.User.Dtos.Response;
using ELearningPlatform.Model.Users.Entity;

namespace ELearningPlatform.Service.Users.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}
