using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(
        UserManager<AppUser> userManager,
        IUnitOfWork unitOfWork,
        IPhotoService photoService) : BaseApiController
{
    [Authorize(Policy = "RequiredAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
            .OrderBy(x => x.UserName)
            .Select(x => new
            {
                x.Id,
                Username = x.UserName,
                Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequiredAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrWhiteSpace(roles)) return BadRequest("You must select at least one role");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);
        if (user is null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratoPhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModeration()
    {
        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratoPhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if (photo is null) return BadRequest("Could not get photo from database");

        photo.IsApproved = true;

        var user = await unitOfWork.UserRepository.GetUserByPhotoId(photoId);

        if (user is null) return BadRequest("Could not get user from database");

        if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

        await unitOfWork.Complete();

        return Ok();
    }


    [Authorize(Policy = "ModeratoPhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if (photo is null) return BadRequest("Could not get photo from database");

        if (photo.PublicId is not null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Result == "ok")
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
        }
        else
        {
            unitOfWork.PhotoRepository.RemovePhoto(photo);
        }
        await unitOfWork.Complete();

        return Ok();
    }

}
