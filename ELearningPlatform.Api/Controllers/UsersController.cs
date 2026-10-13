using Core.Constants;
using ELearningPlatform.Model.User.Dtos.Create;
using ELearningPlatform.Model.User.Dtos.Update;
using ELearningPlatform.Service.Users.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class UsersController(IUserService userService) : CustomBaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
       => CreateActionResult(await userService.CreateUserAsync(createUserDto));

    // Giriş yapmış kullanıcının kendi bilgileri ve rolleri (frontend "ben kimim" için kullanır).
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userName = HttpContext.User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            return Unauthorized();
        }
        return CreateActionResult(await userService.GetUserByNameAsync(userName));
    }

    // Eski adres, geriye dönük uyumluluk için duruyor.
    [Authorize]
    [HttpGet("username")]
    public Task<IActionResult> GetUser() => GetCurrentUser();

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("CreateUserRoles/{userName}")]
    public async Task<IActionResult> CreateUserRoles(string userName, [FromBody] List<string> roles) => CreateActionResult(await userService
        .CreateUserRolesAsync(userName, roles));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
        => CreateActionResult(await userService.GetAllAsync());

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] string id)
        => CreateActionResult(await userService.GetByIdAsync(id));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
        => CreateActionResult(await userService.DeleteAsync(id));

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserRequest request)
        => CreateActionResult(await userService.UpdateAsync(id, request));
}
