using Data.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Web_Student_manager.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorFilterAttribute : TypeFilterAttribute
    {
        public AuthorFilterAttribute(string role) : base(typeof(AuthorFilter))
        {
            Arguments = new object[] { role };
        }
    }
    public class AuthorFilter :  IAsyncAuthorizationFilter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _role;

        public AuthorFilter(IHttpClientFactory httpClientFactory, string role)
        {
            _httpClientFactory = httpClientFactory;
            _role = role;
        }
        

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext; // Lấy HttpContext từ AuthorizationFilterContext
            var httpClient = _httpClientFactory.CreateClient();
            if (httpContext.Session.TryGetValue("JWToken", out byte[] tokenBytes))
            {
                var jwtToken = Encoding.UTF8.GetString(tokenBytes);

                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);

                var response = await httpClient.GetAsync("https://localhost:7164/api/Authorization/GetRole");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse); 

                    if (apiResponse.Message == _role)
                    {
                        // Điều kiện cho vai trò phù hợp
                    }
                    else
                    {
                        context.Result = new UnauthorizedResult(); // Không có vai trò phù hợp, trả về lỗi 401 Unauthorized
                        context.HttpContext.Response.Redirect("/Login/Index"); // Chuyển hướng đến trang đăng nhập
                        return;
                    }
                }
                else
                {
                    context.Result = new UnauthorizedResult(); // Không xác thực, trả về lỗi 401 Unauthorized
                    context.HttpContext.Response.Redirect("/Login/Index"); // Chuyển hướng đến trang đăng nhập
                    return;
                }

            }
            else
            {
                context.Result = new UnauthorizedResult(); // Không xác thực, trả về lỗi 401 Unauthorized
                context.HttpContext.Response.Redirect("/Login/Index"); // Chuyển hướng đến trang đăng nhập
                return;
            }
        }
    }
}
