using AutoMapper;
using Core.Response;
using ELearningPlatform.Model.User.Dtos.Create;
using ELearningPlatform.Model.User.Dtos.Response;
using ELearningPlatform.Model.User.Dtos.Update;
using ELearningPlatform.Model.Users.Entity;
using ELearningPlatform.Service.Users.Abstracts;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace ELearningPlatform.Service.Users.Concretes;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
    }
    public async Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new User
        {
            Email = createUserDto.Email,
            UserName = createUserDto.UserName,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName
        };

        var result = await _userManager.CreateAsync(user, createUserDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description).ToList();
            return ServiceResult<UserDto>.Fail(errors, HttpStatusCode.BadRequest);
        }

        string roleName = createUserDto.Role.ToString(); 
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            return ServiceResult<UserDto>.Fail($"'{roleName}' rolü mevcut değil.");
        }

        var roleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            return ServiceResult<UserDto>.Fail(roleResult.Errors.First().Description);
        }

        var userAsDto = _mapper.Map<UserDto>(user);
        userAsDto.Roles = new List<string> { roleName };
        return ServiceResult<UserDto>.Success(userAsDto, HttpStatusCode.Created);
    }

    public async Task<ServiceResult> CreateUserRolesAsync(string userName, List<string> roles)
    {
        if (roles == null || !roles.Any())
        {
            return ServiceResult.Fail("Atanacak roller belirtilmelidir.");
        }

        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return ServiceResult.Fail("Kullanıcı bulunamadı.", HttpStatusCode.NotFound);
        }
        foreach (var role in roles)
        {

            if (!await _roleManager.RoleExistsAsync(role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole { Name = role });
                if (!roleResult.Succeeded)
                {
                    return ServiceResult.Fail($"Rol oluşturulurken hata oluştu: {role}");
                }
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
            if (!addToRoleResult.Succeeded)
            {
                return ServiceResult.Fail($"Kullanıcıya rol atanırken hata oluştu: {role}");
            }
        }

        return ServiceResult.Success("Roller başarıyla eklendi.", HttpStatusCode.Created);
    }
    public async Task<ServiceResult> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return ServiceResult.Fail("Kullanıcı bulunamadı.",HttpStatusCode.NotFound);
        }
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail(result.Errors.First().Description);
        }
        return ServiceResult.Success("Kullanıcı silindi.", HttpStatusCode.NoContent);
    }
    public async Task<ServiceResult<List<UserDto>>> GetAllAsync()
    {
        var users = _userManager.Users.ToList();
        if (!users.Any())
        {
            return ServiceResult<List<UserDto>>.Fail("Kullanıcı bulunamadı.", HttpStatusCode.NotFound);
        }
        
        var userDtos = _mapper.Map<List<UserDto>>(users);
        return ServiceResult<List<UserDto>>.Success(userDtos, HttpStatusCode.OK);
    }
    public async Task<ServiceResult<UserDto>> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return ServiceResult<UserDto>.Fail("Kullanıcı bulunamadı.",HttpStatusCode.NotFound);
        }
        var roles = await _userManager.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = roles.ToList();
        
        return ServiceResult<UserDto>.Success(userDto,HttpStatusCode.OK);
    }
    public async Task<ServiceResult<UserDto>> GetUserByNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return ServiceResult<UserDto>.Fail("UserName bulunamadı", HttpStatusCode.NotFound);
        }
        var roles = await _userManager.GetRolesAsync(user);
        var userAsDto = _mapper.Map<UserDto>(user);
        userAsDto.Roles = roles.ToList();
        return ServiceResult<UserDto>.Success(userAsDto, HttpStatusCode.OK);
    }
    public async Task<ServiceResult> UpdateAsync(string id, UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            ServiceResult.Fail("Kullanıcı bulunamadı",HttpStatusCode.NotFound);
        }
        user.UserName = request.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded is false)
        {
            ServiceResult.Fail(result.Errors.First().Description);
        }
        return ServiceResult.Success("Kullanıcı güncellendi.", HttpStatusCode.NoContent);
    }
}
