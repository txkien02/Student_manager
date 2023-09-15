using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http;
using Web_Student_manager.Filters;


namespace Web_Student_manager.Controllers
{
    [Route("")]
    [AuthorFilter("User")]
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7164/api/User/"); // Thay đổi địa chỉ API thật của bạn
        }
        [Route("findinfo")]
        // GET: UserController
        public ActionResult Index()
        {
            ViewBag.result = false;
            return View();
        }
        [Route("findinfo")]
        [HttpPost]
        public async Task<ActionResult> Index(string studentID)
        {
            ViewBag.result = false;
            var jwToken = GetTokenFromSession();
            if (string.IsNullOrEmpty(jwToken))
            {
                return RedirectToAction("Login", "Account"); // Hoặc điều hướng đến trang đăng nhập nếu không có token.
            }
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            

            var response = await _httpClient.GetAsync("GetStudent/" + studentID);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<BasicInfoModel>(jsonResponse);

                ViewBag.result = true;
                return View(apiResponse);

            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewBag.result = true;
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
