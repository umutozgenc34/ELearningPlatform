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


    [HttpGet("username")]
    public async Task<IActionResult> GetUser()
    {
        var userName = HttpContext.User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            return NotFound("Kullanıcı adı bulunamadı.");
        }
        return CreateActionResult(await userService.GetUserByNameAsync(userName));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("CreateUserRoles/{userName}")]
    public async Task<IActionResult> CreateUserRoles(string userName, [FromBody] List<string> roles) => CreateActionResult(await userService
        .CreateUserRolesAsync(userName, roles));


    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
        => CreateActionResult(await userService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] string id)
        => CreateActionResult(await userService.GetByIdAsync(id));

    [Authorize(Roles = "Educator,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
        => CreateActionResult(await userService.DeleteAsync(id));

    [Authorize(Roles = "Educator,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserRequest request)
        => CreateActionResult(await userService.UpdateAsync(id, request));

}
