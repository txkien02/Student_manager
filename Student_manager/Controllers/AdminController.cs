using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Student_manager.Models;
using Student_manager.Models.Domain;
using Data.Models.DTO;
using Data.Models;
using System.Net;
using System.Security;
using System.Xml.Linq;

namespace Student_manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly DatabaseContext _context;

        public AdminController(DatabaseContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet("GetAllClass")]
        public async Task<IActionResult> GetClass()
        {
            var listClass = await _context.Classes.ToListAsync();
            return Ok(listClass);
        }
        [HttpGet("GetClass/{id}")]
        public async Task<IActionResult> GetClass(int id)
        {
            var classItem = await _context.Classes.FirstOrDefaultAsync(o => o.Id == id);
            if (classItem != null)
            {
                // Class with the specified ID was found. You can return it or process it further.
                return Ok(classItem);
            }
            else
            {
                // Class with the specified ID was not found. You can return a 404 Not Found response.
                return BadRequest("NotFound");
            }
        }
        [HttpGet("GetStudent/{username}")]
        public async Task<IActionResult> GetStudent(string UserName)
        {
            var User = await userManager.FindByNameAsync(UserName);
            if (User != null)
            {
                // Class with the specified ID was found. You can return it or process it further.
                return Ok(User);
            }
            else
            {
                // Class with the specified ID was not found. You can return a 404 Not Found response.
                return BadRequest("NotFound");
            }
        }
        [HttpPost("CreateClass")]
        public async Task<IActionResult> CreateClass([FromBody] ClassModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "please pass all the valid fields";
                return Ok(status);
            }

            var classe = new Class {
                Name = model.Name,
                TotalStudents = 0

            };
            try
            {
                await _context.Classes.AddAsync(classe);
                var result =  await _context.SaveChangesAsync();
                if(result <= 0)
                {
                    status.StatusCode = 0;
                    status.Message = "Can't save ";
                    return BadRequest(status);
                }
                
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully";
            return Ok(status);
        }

        [HttpPut("EditClass/{id}")]
        public async Task<IActionResult> EditClass(int Id, [FromBody] ClassModel model)
        {

            var status = new Status();

            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the valid fields";
                return Ok(status);
            }

            try
            {
                var existingClass = await _context.Classes.FindAsync(Id);
                if (existingClass == null)
                {
                    status.StatusCode = 0;
                    status.Message = "Class not found";
                    return BadRequest(status);
                }

                existingClass.Name = model.Name;
                // Update other properties as needed

                _context.Classes.Update(existingClass);
                var result  = await _context.SaveChangesAsync();
                if (result <= 0)
                {
                    status.StatusCode = 0;
                    status.Message = "Can't save ";
                    return BadRequest(status);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            status.StatusCode = 1;
            status.Message = "Successfully updated class";
            return Ok(status);
        }

        [HttpDelete("DeleteClass/{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var status = new Status();

            try
            {
                var existingClass = await _context.Classes.FindAsync(id);
                if (existingClass == null)
                {
                    status.StatusCode = 0;
                    status.Message = "Class not found";
                    return BadRequest(status);
                }

                _context.Classes.Remove(existingClass);
                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                {
                    status.StatusCode = 0;
                    status.Message = "Can't save ";
                    return BadRequest(status);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            status.StatusCode = 1;
            status.Message = "Successfully deleted class";
            return Ok(status);
        }

        [HttpGet("SearchStudents")]
        public async Task<IActionResult> SearchStudents(string? UserName, string? Name, int? ClassID)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync("User"); // Replace "User" with the actual role name in your system

            if (ClassID != 0 && ClassID!=null)
            {
                usersInRole = usersInRole.Where(s => s.ClassID == ClassID).ToList();
            }
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                usersInRole = usersInRole.Where(s => s.UserName.Contains(UserName)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                usersInRole = usersInRole.Where(s => s.Name.ToLower().Contains(Name.ToLower())).ToList();
            }

            return Ok(usersInRole);
        }

        [HttpPost("AddStudent")]
        public async Task<IActionResult> AddStudent([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return BadRequest(status);
            }

            var resultClass = await _context.Classes.FirstOrDefaultAsync(o=>o.Id==model.ClassID);
            if (resultClass == null)
            {
                status.StatusCode=0;
                status.Message = "Class not Found";
                return BadRequest(status);
            }

            Random random = new Random();
            async Task<string> GenerateRandomUserNameAsync()
            {
                string randomNumber = random.Next(10000).ToString("D4");
                string userName = model.ClassID.ToString() + model.Gender + DateTime.Now.Year.ToString() + randomNumber;
                var userExists = await userManager.FindByNameAsync(userName);
                if (userExists != null)
                {
                    return await GenerateRandomUserNameAsync(); // Nếu tên người dùng đã tồn tại, tạo tên ngẫu nhiên mới
                }
                return userName;
            }


            model.UserName = await GenerateRandomUserNameAsync();


            var user = new ApplicationUser
            {
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Email = model.Email,
                DOB = model.DOB,
                Gender = model.Gender,
                Address = model.Address,
                Status = model.Status,
                Avatar = model.Avatar,
                ClassID = model.ClassID
            };
            // create a user here
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return BadRequest(status);
            }

            // add roles here
            // for admin registration UserRoles.Admin instead of UserRoles.Roles
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await userManager.AddToRoleAsync(user, UserRoles.User);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";
            return Ok(status);

        }

        [HttpPut("EditStudent/{username}")]
        public async Task<IActionResult> EditStudent(string UserName, [FromBody] RegistrationModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(UserName);
            if (user != null)
            {
                user.Name = model.Name;
                user.Email = model.Email;
                user.DOB = model.DOB;
                user.Gender = model.Gender;
                user.Address = model.Address;
                user.Status = model.Status;
                user.Avatar = model.Avatar;
                user.ClassID = model.ClassID;
                //model.Password=123456
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(user, model.Password);

                }

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    status.StatusCode = 0;
                    status.Message = "Edit failed";
                    return BadRequest(status);
                }
                else
                {
                    status.StatusCode = 1;
                    status.Message = "Sucessfully Edit";
                    return Ok(status);
                }
            }


            status.StatusCode = 0;
            status.Message = "Invalid Username";
            return Ok(status);
        }

        [HttpDelete("DeleteStudent/{UserName}")]
        public async Task<IActionResult> DeleteStudent(string UserName)
        {
            var status = new Status();

            var user = await userManager.FindByNameAsync(UserName);
                if (user == null)
                {
                    status.StatusCode = 0;
                    status.Message = "Student not found";
                    return BadRequest(status);
                }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "Sucessfully Edit";
                return BadRequest(status);  
            }

            status.StatusCode = 1;
            status.Message = "Successfully deleted class";
            return Ok(status);
        }


    }
}
