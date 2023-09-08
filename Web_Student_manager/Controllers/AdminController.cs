using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http;
using System.Text;
using Web_Student_manager.Filters;

namespace Web_Student_manager.Controllers
{

    [AuthorFilterFilter("Admin")]
    public class AdminController : Controller
    {
        // GET: AdminController
        private readonly HttpClient _httpClient;

        public AdminController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7164/api/Admin"); // Thay đổi địa chỉ API thật của bạn
        }
        public async Task<ActionResult> Index()
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
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClassModel newClass)
        {
            if (ModelState.IsValid)
            {
                // Thêm lớp học vào cơ sở dữ liệu
                // Cập nhật tổng số học sinh sau khi thêm
                // Redirect đến trang danh sách lớp học
            }
            return View(newClass);
        }

        // Action để sửa thông tin lớp học
        [HttpPost]
        public IActionResult Edit(ClassModel editedClass)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin lớp học trong cơ sở dữ liệu
                // Redirect đến trang danh sách lớp học
            }
            return View(editedClass);
        }

        


    }
}
