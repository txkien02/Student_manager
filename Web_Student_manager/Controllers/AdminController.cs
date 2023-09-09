using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.Data;
using System.Net.Http;
using System.Reflection;
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
            _httpClient.BaseAddress = new Uri("https://localhost:7164/api/Admin/"); // Thay đổi địa chỉ API thật của bạn
        }
        public async Task<ActionResult> Index()
        {

            var jwToken = GetTokenFromSession();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);


            var response = await _httpClient.GetAsync("GetAllClass");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<IEnumerable<ClassModel>>(jsonResponse);

                return View(apiResponse);

            }
            return View(null);
        }

        public IActionResult CreateClass()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass(ClassModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về view với thông tin lỗi nếu ModelState không hợp lệ.
            }

            var jwToken = GetTokenFromSession();
            if (string.IsNullOrEmpty(jwToken))
            {
                return RedirectToAction("Login", "Account"); // Hoặc điều hướng đến trang đăng nhập nếu không có token.
            }

            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("CreateClass", content);

            if (response.IsSuccessStatusCode)
            {
                // Xử lý khi tạo lớp học thành công
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý khi có lỗi từ API
                // Ví dụ: lấy thông báo lỗi từ API
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ModelState.AddModelError(string.Empty, apiResponse.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> EditClass(int id)
        {
            var jwToken = GetTokenFromSession();
            if (string.IsNullOrEmpty(jwToken))
            {
                return RedirectToAction("Login", "Account"); // Hoặc điều hướng đến trang đăng nhập nếu không có token.
            }
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var response = await _httpClient.GetAsync("GetClass/"+id);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ClassModel>(jsonResponse);
                return View(apiResponse);
                
            }
            return RedirectToAction("Index");
        }

        // Action để sửa thông tin lớp học
        [HttpPost]
        public async Task<IActionResult> EditClass(ClassModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về view với thông tin lỗi nếu ModelState không hợp lệ.
            }

            var jwToken = GetTokenFromSession();
            if (string.IsNullOrEmpty(jwToken))
            {
                return RedirectToAction("Login", "Account"); // Hoặc điều hướng đến trang đăng nhập nếu không có token.
            }
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("EditClass/" + model.Id, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ModelState.AddModelError(string.Empty, apiResponse.Message);
                return View(model);
            }

            
        }

        public async Task<IActionResult> DeleteClass(int id)
        {
            var jwToken = GetTokenFromSession();
            if (string.IsNullOrEmpty(jwToken))
            {
                return RedirectToAction("Login", "Account"); // Hoặc điều hướng đến trang đăng nhập nếu không có token.
            }
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var response = await _httpClient.DeleteAsync("DeleteClass/" + id);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");

            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ModelState.AddModelError(string.Empty, apiResponse.Message);
                return View();
            }

            
        }

        private string GetTokenFromSession()
        {
            // Lấy token từ session bằng cách sử dụng IHttpContextAccessor
            var session = HttpContext.Session;
            var token = session.GetString("JWToken");

            return string.IsNullOrEmpty(token) ? string.Empty : token;
        }
    }   


    
}
