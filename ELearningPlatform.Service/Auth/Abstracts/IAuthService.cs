using Core.Response;
using ELearningPlatform.Model.Token;
using ELearningPlatform.Model.User.Dtos.Login;

namespace ELearningPlatform.Service.Auth.Abstracts;

public interface IAuthService
{
    Task<ServiceResult<TokenDto>> CreateTokenAsync(LoginDto loginDto);
}
