﻿using Core.Options;
using Core.Services;
using ELearningPlatform.Model.Token;
using ELearningPlatform.Model.Users.Entity;
using ELearningPlatform.Service.Token.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ELearningPlatform.Service.Token.Concretes;

public class TokenService : ITokenService
{
    private readonly UserManager<User> _userManager;
    private readonly CustomTokenOption _tokenOption;
    public TokenService(UserManager<User> userManager, IOptions<CustomTokenOption> options)
    {
        _userManager = userManager;
        _tokenOption = options.Value;
    }
    public async Task<TokenDto> CreateTokenAsync(User user)
    {
        var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
        var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);
        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        var claims = await GetClaims(user, _tokenOption.Audience);
        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
            issuer: _tokenOption.Issuer,
            expires: accessTokenExpiration,
            notBefore: DateTime.Now,
            claims: claims,
            signingCredentials: signingCredentials);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(jwtSecurityToken);
        var tokenDto = new TokenDto(
            AccessToken: token,
            AccessTokenExpiration: accessTokenExpiration
        );
        return tokenDto;
    }

    private async Task<IEnumerable<Claim>> GetClaims(User user, List<string> audiences)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var userList = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name,user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
        userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
        userList.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));
        return userList;
    }

}