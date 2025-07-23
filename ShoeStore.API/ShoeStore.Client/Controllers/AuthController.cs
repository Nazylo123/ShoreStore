using Microsoft.AspNetCore.Mvc;
using ShoeStore.Client.Models;
using ShoeStore.Client.Services;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ShoeStore.Client.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var loginData = new
                {
                    Username = model.Username,
                    Password = model.Password
                };

                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_configuration["ApiBaseUrl"]}auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResult = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (loginResult?.Token != null)
                    {
                        // Store token in session
                        HttpContext.Session.SetString("JwtToken", loginResult.Token);
                        HttpContext.Session.SetString("Username", model.Username);
                        HttpContext.Session.SetString("Role", loginResult.Role ?? "Customer");
                        HttpContext.Session.SetString("UserId", loginResult.CustomerId.ToString());

                        // Thiết lập authentication cookie
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, model.Username),
                            new Claim(ClaimTypes.Role, loginResult.Role ?? "Customer")
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                        };
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties
                        );

                        TempData["SuccessMessage"] = "Đăng nhập thành công!";
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var registerData = new
                {
                    Username = model.Username,
                    Password = model.Password,
                    Email = model.Email,
                    FullName = model.FullName, // Đúng tên trường backend yêu cầu
                    Phone = model.Phone,
                    Address = model.Address ?? "Chưa cập nhật"
                };

                var json = JsonSerializer.Serialize(registerData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_configuration["ApiBaseUrl"]}auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorDict = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                    if (errorDict != null && errorDict.ContainsKey("errors"))
                    {
                        var errors = errorDict["errors"] as JsonElement?;
                        if (errors.HasValue)
                        {
                            foreach (var prop in errors.Value.EnumerateObject())
                            {
                                foreach (var err in prop.Value.EnumerateArray())
                                {
                                    ModelState.AddModelError(prop.Name, err.GetString());
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Đăng ký thất bại. Vui lòng thử lại.");
                        }
                    }
                    else if (errorDict != null && errorDict.ContainsKey("message"))
                    {
                        ModelState.AddModelError("", errorDict["message"].ToString());
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đăng ký thất bại. Vui lòng thử lại.");
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "Đăng ký thất bại. Vui lòng thử lại.");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đăng ký. Vui lòng thử lại.");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{_configuration["ApiBaseUrl"]}auth/profile");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            var json = await response.Content.ReadAsStringAsync();
            var profile = System.Text.Json.JsonSerializer.Deserialize<UserProfileViewModel>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(profile);
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public int CustomerId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
} 