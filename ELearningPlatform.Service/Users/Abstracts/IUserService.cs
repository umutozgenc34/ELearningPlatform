using Core.Response;
using ELearningPlatform.Model.User.Dtos.Create;
using ELearningPlatform.Model.User.Dtos.Response;
using ELearningPlatform.Model.User.Dtos.Update;

namespace ELearningPlatform.Service.Users.Abstracts;

public interface IUserService
{
    Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<ServiceResult<UserDto>> GetUserByNameAsync(string userName);
    Task<ServiceResult> CreateUserRolesAsync(string userName, List<string> roles);
    Task<ServiceResult<List<UserDto>>> GetAllAsync();
    Task<ServiceResult<UserDto>> GetByIdAsync(string id);
    Task<ServiceResult> UpdateAsync(string id, UpdateUserRequest request);
    Task<ServiceResult> DeleteAsync(string id);
}
