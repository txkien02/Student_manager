using Data.Models;
using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Web_Student_manager.Filters;

namespace Web_Student_manager.Controllers
{

    [AuthorFilter("Admin")]
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
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;
                return View(null);

            }
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
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;
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
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;
                return RedirectToAction("Index");
            }
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
                ViewData["error"] = apiResponse.Message;
                return View(model);
            }

            
        }

        [HttpPost]
        public async Task<IActionResult> Index(int id)
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

        public async Task<IActionResult> Student_Index()
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

        [HttpPost]
        public async Task<IActionResult> Student_Index(string searchOption, string searchValue, int classId)
        {
            string param ="";
            if(searchOption== "studentId")
            {
                param += "UserName=" + searchValue;
            }
            else
            {
                param += "&Name=" + searchValue;
            }
            if(classId!= 0)
            {
                param += "&ClassID=" + classId;
            }

            var jwToken = GetTokenFromSession();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var response = await _httpClient.GetAsync("SearchStudents?" + param);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<IEnumerable<RegistrationModel>>(jsonResponse);
                ViewData["Student"] = apiResponse;



                var classes = await getclasss();
                return View(classes);
            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;
                return View((IEnumerable <ClassModel> )getclasss());
            }
           
        }

        public async Task<IActionResult> CreateStudent()
        {
            var jwToken = GetTokenFromSession();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var getclass = await getclasss();
            ViewData["Class"] = (IEnumerable <Data.Models.DTO.ClassModel>) getclass;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(RegistrationModel model)
        {
            IFormFile img = Request.Form.Files["img"];
            if (img != null && img.Length > 0)
            {
                // Đọc dữ liệu từ tệp hình ảnh và chuyển đổi thành mảng byte
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    img.CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
                model.Avatar = imageBytes;
            }

            var jwToken = GetTokenFromSession();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("AddStudent",content);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("Student_Index");
            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;
                
                return View((IEnumerable<ClassModel>)getclasss());
            }


            
        }

        public async Task<IActionResult> EditStudent(string UserName)
        {

            var jwToken = GetTokenFromSession();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);


            var response = await _httpClient.GetAsync("GetStudent/" + UserName);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<RegistrationModel>(jsonResponse);
                
                var getclass = await getclasss();
                ViewData["Class"] = (IEnumerable<Data.Models.DTO.ClassModel>)getclass;
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
        [HttpPost]
        public async Task<IActionResult> EditStudent(RegistrationModel model) {

            

            IFormFile img = Request.Form.Files["img"];
            if (img != null && img.Length > 0)
            {
                // Đọc dữ liệu từ tệp hình ảnh và chuyển đổi thành mảng byte
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    img.CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
                model.Avatar = imageBytes;
            }

            
            var jwToken = GetTokenFromSession();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);
            var response = await _httpClient.PutAsync("EditStudent/"+model.UserName, content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Student_Index");
            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;
                var getclass = await getclasss();
                ViewData["Class"] = (IEnumerable<Data.Models.DTO.ClassModel>)getclass;
                return View(model);
            }

        }



        

        [HttpPost]
        public async Task<IActionResult> DeleteStudent(string UserName)
        {
            var jwToken = GetTokenFromSession();
            if (string.IsNullOrEmpty(jwToken))
            {
                return RedirectToAction("Login", "Account"); // Hoặc điều hướng đến trang đăng nhập nếu không có token.
            }
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);

            var response = await _httpClient.DeleteAsync("DeleteStudent/" + UserName);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Student_Index");

            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);
                ViewData["error"] = apiResponse.Message;

                return RedirectToAction("Student_Index");
            }


        }
        private async Task<IEnumerable<ClassModel>> getclasss()
        {

            var response = await _httpClient.GetAsync("GetAllClass");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<IEnumerable<ClassModel>>(jsonResponse);

                return apiResponse;

            }
            return null;
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
