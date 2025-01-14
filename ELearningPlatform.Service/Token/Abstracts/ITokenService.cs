using ELearningPlatform.Model.Token;
using ELearningPlatform.Model.Users.Entity;

namespace ELearningPlatform.Service.Token.Abstracts;

public interface ITokenService
{
    Task<TokenDto> CreateTokenAsync(User user);
}
