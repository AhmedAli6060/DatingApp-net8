using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper) : BaseApiController
{

    [HttpPost("register")] //account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data");

            if (await UserExists(registerDto.Username)) return BadRequest($"username is taken");


            var user = mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();//

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
                return BadRequest("Invalid data");

            var user = await userManager.Users
                                .Include(p => p.Photos)
                                .FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.Username.ToUpper());

            if (user is null || user.UserName is null) return Unauthorized("Invalid username.");

            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized("Wrong password!");

            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = await tokenService.CreateToken(user),
                Gender = user.Gender,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    private async Task<bool> UserExists(string userName) =>
        await userManager.Users.AnyAsync(x => x.NormalizedUserName == userName.ToUpper());

}
