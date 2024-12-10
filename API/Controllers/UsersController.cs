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
    public class UsersController(IUserRepository userRepository) : BaseApiController
    {

        #region  Methods

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();
            return Ok(users);
        }

        // [HttpGet("{id}")]
        // public async Task<ActionResult<AppUser>> GetUser(int id)
        // {
        //     var user = await userRepository.GetUserByIdAsync(id);
        //     if (user is null)
        //         return NotFound($"User with id {id} not exist");

        //     return Ok(new { user });
        // }

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
                return Ok(new { recentlyAdded });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> EditUser(int id, AppUser model)
        {
            try
            {
                var user = await userRepository.GetUserByIdAsync(id);
                if (user is null)
                    return NotFound("user not found");

                user.UserName = model.UserName;
                userRepository.Update(user);
                await userRepository.SaveAllAsync();

                return Ok(new { user });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
