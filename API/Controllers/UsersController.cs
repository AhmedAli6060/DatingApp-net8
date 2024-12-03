using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController(DataContext context) : BaseApiController
    {

        #region  Methods

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await context.Users.ToListAsync();
            return Ok(new { users });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user is null)
                return NotFound($"User with id {id} not exist");

            return Ok(new { user });
        }

        [HttpPost]
        public ActionResult<AppUser> AddUser(AppUser model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data");

                context.Users.Add(model);
                context.SaveChanges();

                var recentlyAdded = context.Users.Find(model.Id);
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
                var user = await context.Users.FindAsync(id);
                if (user is null)
                    return NotFound("user not found");

                user.UserName = model.UserName;
                context.Update(user);
                await context.SaveChangesAsync();

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
                var user = await context.Users.FindAsync(id);
                if (user is null)
                    return NotFound("user not found");

                context.Remove(user);
                await context.SaveChangesAsync();
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
