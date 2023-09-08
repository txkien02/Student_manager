using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Web_Student_manager.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddHttpClient();

// Cấu hình xác thực bằng JWT Token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    }).AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
    }); ;

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(20);

});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAuthenticatedUser(); // Yêu cầu người dùng đã xác thực

        // Kiểm tra vai trò từ Session
        policy.RequireAssertion(context =>
        {
            var userRole = context.User.FindFirstValue("UserRole"); // Lấy vai trò từ thông tin xác thực
            if (!string.IsNullOrEmpty(userRole) && userRole == "Admin")
            {
                return true; // Người dùng có vai trò "Admin"
            }
            return false; // Người dùng không có vai trò "Admin"
        });
    });
    options.AddPolicy("User", policy =>
    {
        policy.RequireAuthenticatedUser(); // Yêu cầu người dùng đã xác thực

        // Kiểm tra vai trò từ Session
        policy.RequireAssertion(context =>
        {
            var userRole = context.User.FindFirstValue("UserRole"); // Lấy vai trò từ thông tin xác thực
            if (!string.IsNullOrEmpty(userRole) && (userRole == "User"))
            {
                return true; // Người dùng có vai trò "Admin" hoặc "User"
            }
            return false; // Người dùng không có vai trò "Admin" hoặc "User"
        });
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Index}/{id?}"); ;

app.Run();
