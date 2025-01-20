using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService) : BaseApiController
    {

        #region  Methods

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            userParams.CurrentUserName = User.GetUserName();
            var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users);
            return Ok(users);
        }

        [HttpGet("{username}")] // / api/users/2
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await unitOfWork.UserRepository.GetMemberAsync(username);
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

                unitOfWork.UserRepository.Add(model);
                await unitOfWork.Complete();

                var recentlyAdded = await unitOfWork.UserRepository.GetUserByIdAsync(model.Id);
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
                var user = await unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());

                if (user is null) return BadRequest("Could not find user");

                mapper.Map(model, user);
                if (await unitOfWork.Complete()) return NoContent();

                return BadRequest("Failed to update the user");
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
                var user = await unitOfWork.UserRepository.GetUserByIdAsync(id);
                if (user is null)
                    return NotFound("user not found");

                unitOfWork.UserRepository.Delete(user);
                await unitOfWork.Complete();
                return Ok("User deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user is null) return BadRequest("Cannot update user");

            var result = await photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await unitOfWork.Complete())
                return CreatedAtAction(nameof(GetUser),
                    new { username = user.UserName }, mapper.Map<PhotoDto>(photo));
            //return mapper.Map<PhotoDto>(photo);

            return BadRequest("Problem adding photo");
        }


        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMAinPhoto(int photoId)
        {
            var user = await unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user is null) return BadRequest("Could not find user");

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo is null || photo.IsMain) return BadRequest("Cannot use this as main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await unitOfWork.Complete()) return NoContent();

            return BadRequest("Problem setting main photo");

        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());
            if (user is null) return BadRequest("Could not find user");

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo is null || photo.IsMain) return BadRequest("This photo can not be deleted");

            if (!string.IsNullOrWhiteSpace(photo.PublicId))
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleteing photo");
        }

        #endregion

    }
}
