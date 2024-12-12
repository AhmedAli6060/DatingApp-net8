using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
    {

        #region  Methods

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await userRepository.GetMemberAsync(username);
            if (user is null)
                return NotFound($"User with name {username} not exist");

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<AppUser>> AddUser(AppUser model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data");

                userRepository.Add(model);
                await userRepository.SaveAllAsync();

                var recentlyAdded = await userRepository.GetUserByIdAsync(model.Id);
                return Ok(recentlyAdded);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto model)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (username is null) return BadRequest("No username found in token");

                var user = await userRepository.GetUserByUserNameAsync(username);

                if (user is null) return BadRequest("Could not find user");

                mapper.Map(model, user);
                if(await userRepository.SaveAllAsync()) return NoContent();

                return BadRequest("Failed to update the user");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // [HttpPut("{id}")]
        // public async Task<ActionResult> EditUser(int id, MemberDto model)
        // {
        //     try
        //     {
        //         var user = await userRepository.GetUserByIdAsync(id);
        //         if (user is null)
        //             return NotFound("user not found");

        //         //user.UserName = model.UserName;
        //         user.Introduction = model.Introduction;
        //         user.LookingFor = model.LookingFor;
        //         user.Interests=model.Interests;
        //         user.City=model.City;
        //         user.Country=model.Country;
        //         userRepository.Update(user);
        //         await userRepository.SaveAllAsync();

        //         return Ok(user);
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var user = await userRepository.GetUserByIdAsync(id);
                if (user is null)
                    return NotFound("user not found");

                userRepository.Delete(user);
                await userRepository.SaveAllAsync();
                return Ok("User deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

    }
}
