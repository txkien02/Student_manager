using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Student_manager.Models;
using Student_manager.Models.Domain;
using Data.Models.DTO;
using Student_manager.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography.X509Certificates;
using Data.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Student_manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthorizationController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenService _tokenService;
        public AuthorizationController(DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService
            )
        {
            this._context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._tokenService = tokenService;
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            string UserName = User.Identity.Name;
            var status = new Status();
            // check validations
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "please pass all the valid fields";
                return Ok(status);
            }
            // lets find the user
            var user = await userManager.FindByNameAsync(UserName);
            if(user is null)
            {
                status.StatusCode = 0;
                status.Message = "invalid username";
                return Ok(status);
            }
            // check current password
            if(!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                status.StatusCode = 0;
                status.Message = "invalid current password";
                return Ok(status);
            }

            // change password here
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "Failed to change password";
                return Ok(status);
            }
            status.StatusCode = 1;
            status.Message = "Password has changed successfully";
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = _tokenService.GetToken(authClaims);
                var refreshToken = _tokenService.GetRefreshToken();
                var tokenInfo = _context.TokenInfo.FirstOrDefault(a => a.Usename == user.UserName);
                if (tokenInfo == null)
                {
                    var info = new TokenInfo
                    {
                        Usename = user.UserName,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiry = DateTime.Now.AddDays(1)
                    };
                    _context.TokenInfo.Add(info);
                }

                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.RefreshTokenExpiry = DateTime.Now.AddDays(1);
                }
                try
                {
                    //fix
                    _context.SaveChanges();
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                var fuser = await userManager.FindByNameAsync(user.UserName);
                var role = await userManager.GetRolesAsync(fuser);
                return Ok(new LoginResponse
                {
                    Name = user.Name,
                    Username = user.UserName,
                    Role = role[0],
                    Token = token.TokenString,
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo,
                    StatusCode = 1,
                    Message = "Logged in"
                }); 

            }
            //login failed condition

            return Ok(
                new LoginResponse {
                    StatusCode = 0,
                    Message = "Invalid Username or Password",
                    Token = "", Expiration = null });
        }

        [HttpPost("Registration")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Registration([FromBody]RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
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

            //// check if user exists
            //var userExists = await userManager.FindByNameAsync(model.UserName);
            //if (userExists!=null)
            //{
            //    status.StatusCode = 0;
            //    status.Message = "Invalid username";
            //    return Ok(status);
            //}



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
            var result= await userManager.CreateAsync(user, model.Password); 
            if(!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return Ok(status);
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

        //after registering admin we will comment this code, because i want only one admin in this application
        [HttpPost("RegistrationAdmin")]
        public async Task<IActionResult> RegistrationAdmin([FromBody] RegistrationAdminModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
            }
            // check if user exists
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return Ok(status);
            }
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
                ClassID = model.ClassID,
            };
            // create a user here
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";    
                return Ok(status);
            }

            // add roles here
            // for admin registration UserRoles.Admin instead of UserRoles.Roles
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";
            return Ok(status);

        }

        [HttpGet]
        [Route("getrole")]
        [Authorize]
        public async Task<IActionResult> GetRole()
        {
            var status = new Status();
            status.StatusCode = 1;
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var role = await userManager.GetRolesAsync(user);
            status.Message = role[0].ToString();
            return Ok(status);
        }

    }
}
