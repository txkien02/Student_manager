using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Student_manager.Models.Domain;
using Student_manager.Repositories.Abstract;
using System.Data;

namespace Student_manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NavController : ControllerBase
    {
        
        private readonly ITokenService _service;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DatabaseContext _context;

        public NavController(DatabaseContext context, UserManager<ApplicationUser> userManager, ITokenService service)
        {
            this._context = context;
            this.userManager = userManager;
            this._service = service;
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
        public async Task<IActionResult> ChangeInfo(ChangeInfoModel model)
        {
            string UserName = User.Identity.Name;
            var status = new Status();
            var user = await userManager.FindByNameAsync(UserName);
            if (user != null)
            {
                if(model.Avatar!= null)
                {

                    user.Avatar = model.Avatar; 
                }
                user.Address = model.Address;

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

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var status = new Status();
            string UserName = User.Identity.Name;
            // check validations
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "please pass all the valid fields";
                return BadRequest(status);
            }
            // lets find the user
            var user = await userManager.FindByNameAsync(UserName);
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


        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            try
            {
                var username = User.Identity.Name;
                var user = _context.TokenInfo.SingleOrDefault(u => u.Usename == username);
                if (user is null)
                    return BadRequest();
                user.RefreshToken = "";
                user.RefreshTokenExpiry = DateTime.UtcNow;
                _context.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
