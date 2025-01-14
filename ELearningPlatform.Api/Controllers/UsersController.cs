using ELearningPlatform.Model.User.Dtos.Create;
using ELearningPlatform.Model.User.Dtos.Update;
using ELearningPlatform.Service.Users.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

public class UsersController(IUserService userService) : CustomBaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
       => Ok(await userService.CreateUserAsync(createUserDto));

    [HttpGet("username")]
    public async Task<IActionResult> GetUser()
    {
        var userName = HttpContext.User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            return NotFound("Kullanıcı adı bulunamadı.");
        }
        return Ok(await userService.GetUserByNameAsync(userName));
    }

    [HttpPost("CreateUserRoles/{userName}")]
    public async Task<IActionResult> CreateUserRoles(string userName, [FromBody] List<string> roles) => Ok(await userService
        .CreateUserRolesAsync(userName, roles));


    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
        => Ok(await userService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] string id)
        => Ok(await userService.GetByIdAsync(id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
        => Ok(await userService.DeleteAsync(id));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserRequest request)
        => Ok(await userService.UpdateAsync(id, request));

}
