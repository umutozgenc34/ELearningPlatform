using ELearningPlatform.Model.User.Dtos.Login;
using ELearningPlatform.Service.Auth.Abstracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class AuthsController(IAuthService authService) : CustomBaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto) => Ok(await authService.CreateTokenAsync(loginDto));

}
