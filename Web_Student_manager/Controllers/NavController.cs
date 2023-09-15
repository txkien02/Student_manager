using Data.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Web_Student_manager.Filters;

namespace Web_Student_manager.Controllers
{
    [AuthorFilter]
    [Route("")]
    public class NavController : Controller
    {
        private readonly HttpClient _httpClient;
        public NavController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7164/api/Nav/"); // Thay đổi địa chỉ API thật của bạn
        }
        // GET: NavController
        [Route("info")]
        public async Task<IActionResult> Index()
        {
            var jwToken = GetTokenFromSession();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);


            var response = await _httpClient.GetAsync("GetInfo");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<RegistrationModel>(jsonResponse);

               
                return View(apiResponse);

            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;
                return View(null);

            }
        }
        [Route("info")]
        [HttpPost]
        public async Task<IActionResult> Index(string address, IFormFile img)
        {
            var info = new ChangeInfoModel();
            byte[] img_data = null;
            if (img != null && img.Length > 0)
            {
                // Đọc dữ liệu từ tệp hình ảnh và chuyển đổi thành mảng byte
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    img.CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
                img_data = imageBytes;
                info = new ChangeInfoModel
                {
                    Avatar = img_data,
                    Address = address,
                };
            }
            else
            {
                info = new ChangeInfoModel
                {
                    Address = address
                };

            }

            var jwToken = GetTokenFromSession();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var content = new StringContent(JsonConvert.SerializeObject(info), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("ChangeInfo", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;
                
                return View();
            }
            

        }
        [Route("changepassword")]
        public IActionResult ChangePassword()
        {
            var password = new ChangePasswordModel();
            return View(password);
        }

        [Route("changepassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {

            var jwToken = GetTokenFromSession();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("ChangePassword", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;

                return View(model);
            }
        }
        [Route("logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var jwToken = GetTokenFromSession();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var response = await _httpClient.PostAsync("Logout",null);
            if (response.IsSuccessStatusCode)
            {
                
                HttpContext.Session.Clear();
                return RedirectToAction("Index","Login");
            }else return RedirectToAction("Index", "Login");
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
