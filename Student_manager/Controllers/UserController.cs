using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Student_manager.Models.Domain;
using Data.Models.DTO;
using System.Data;
using Student_manager.Models.DTO;

namespace Student_manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly DatabaseContext _context;

        public UserController(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            this._context = context;
            this.userManager = userManager;
        }

        [HttpGet("GetStudent/{UserName}")]
        public async Task<IActionResult> GetStudent(string UserName)
        {


            var status = new Status();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Student not found";
                return BadRequest(status);
            }

            var info = new BasicInfoModel
            {
                Name = user.Name,
                Email = user.Email,
                DOB = user.DOB,
                Address = user.Address,
                Status = user.Status,
                Avatar = user.Avatar,
                ClassID = user.ClassID
            };
            return Ok(info);

        }
        [HttpGet("GetInfo")]
        public async Task<IActionResult> GetInfo()
        {
            string UserName = User.Identity.Name;
            var status = new Status();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Student not found";
                return BadRequest(status);
            }

            return Ok(user);

        }

        [HttpPut("ChangeInfo")]
        public async Task<IActionResult> ChangeInfo( ChangeInfoModel model)
        {
            string UserName = User.Identity.Name;
            var status = new Status();
            var user = await userManager.FindByNameAsync(UserName);
            if (user != null)
            {
                user.Address = model.Address;
                user.Avatar = model.Avatar;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    status.StatusCode = 0;
                    status.Message = "Change failed";
                    return BadRequest(status);
                }
                else
                {
                    
                    status.StatusCode = 1;
                    status.Message = "Sucessfully Change";
                    return Ok(status);
                }
            }   

            status.StatusCode = 0;
            status.Message = "Student not found";
            return BadRequest(status);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var status = new Status();
            // check validations
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "please pass all the valid fields";
                return BadRequest(status);
            }
            // lets find the user
            var user = await userManager.FindByNameAsync(model.Username);
            if (user is null)
            {
                status.StatusCode = 0;
                status.Message = "invalid username";
                return BadRequest(status);
            }
            // check current password
            if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                status.StatusCode = 0;
                status.Message = "invalid current password";
                return BadRequest(status);
            }

            // change password here
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "Failed to change password";
                return BadRequest(status);
            }
            status.StatusCode = 1;
            status.Message = "Password has changed successfully";
            return Ok(result);
        }
    }
}
