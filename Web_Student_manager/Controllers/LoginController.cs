﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Models.DTO;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Reflection;

namespace Web_Student_manager.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var jwToken = GetTokenFromSession();
            if (!string.IsNullOrEmpty(jwToken))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwToken);
                var response = await httpClient.GetAsync("https://localhost:7164/api/Authorization/GetRole");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<Status>(jsonResponse);

                 
                    if (apiResponse.Message == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (apiResponse.Message == "User")
                    {
                            return RedirectToAction("Index", "User");
                    }

                }
            }

            return View();
        }


        public async Task<IActionResult> Login()
        {
            // Tạo một instance của HttpClient sử dụng IHttpClientFactory
            var httpClient = _httpClientFactory.CreateClient();

            // Tạo model và gửi yêu cầu POST đến API
            var model = new LoginModel
            {
                Username = HttpContext.Request.Form["Username"].ToString(),
                Password = HttpContext.Request.Form["Password"].ToString()
            };

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://localhost:7164/api/Authorization/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);

                // Kiểm tra StatusCode trong phản hồi
                if (apiResponse.StatusCode == 1)
                {
                    // Lưu trữ JWT Token và Refresh Token vào session
                    HttpContext.Session.SetString("JWToken", apiResponse.Token);
                    HttpContext.Session.SetString("RefreshToken", apiResponse.RefreshToken);

                    // Lưu thông tin người dùng vào session
                    HttpContext.Session.SetString("UserName", apiResponse.Username);
                    HttpContext.Session.SetString("UserRole", apiResponse.Role);

                    if (apiResponse.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (apiResponse.Role == "User")
                    {
                        return RedirectToAction("Index", "User");
                    }
                }
                else
                {
                    // Xử lý trường hợp đăng nhập thất bại
                    ViewBag.Message = apiResponse.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Xử lý trường hợp không thành công khi gửi yêu cầu đăng nhập
                ViewBag.Message = "Có lỗi xảy ra khi đăng nhập.";
                return RedirectToAction("Index");
            }

            return View();
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
