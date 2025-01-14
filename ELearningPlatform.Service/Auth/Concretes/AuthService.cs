using Core.Response;
using ELearningPlatform.Model.Token;
using ELearningPlatform.Model.User.Dtos.Login;
using ELearningPlatform.Model.Users.Entity;
using ELearningPlatform.Repository.UnitOfWorks.Abstracts;
using ELearningPlatform.Service.Auth.Abstracts;
using ELearningPlatform.Service.Token.Abstracts;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace ELearningPlatform.Service.Auth.Concretes;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(ITokenService tokenService, UserManager<User> userManager, IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }
    public async Task<ServiceResult<TokenDto>> CreateTokenAsync(LoginDto loginDto)
    {
        if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return ServiceResult<TokenDto>.Fail("Email veya  Password yanlış", HttpStatusCode.BadRequest);
        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return ServiceResult<TokenDto>.Fail("Email veya Password yanlış", HttpStatusCode.BadRequest);
        }
        var token = await _tokenService.CreateTokenAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<TokenDto>.Success(token, HttpStatusCode.Created);
    }
}