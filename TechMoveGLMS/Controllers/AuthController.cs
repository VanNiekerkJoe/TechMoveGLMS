using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TechMoveGLMS.Services;

namespace TechMoveGLMS.Controllers
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
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001";
            client.BaseAddress = new Uri(apiBaseUrl);

            var loginData = new { username = model.Username, password = model.Password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("/api/auth/login", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode && result?.Success == true)
                {
                    // Store token in session
                    HttpContext.Session.SetString("AuthToken", result.Token ?? "");
                    HttpContext.Session.SetString("Username", model.Username);
                    HttpContext.Session.SetString("UserRole", result.User?.Role ?? "User");
                    HttpContext.Session.SetString("UserFullName", result.User?.FullName ?? model.Username);

                    // Create cookie authentication
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, result.User?.Role ?? "User"),
                        new Claim("FullName", result.User?.FullName ?? model.Username)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    // Redirect to return URL or home
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", result?.Message ?? "Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error connecting to API: {ex.Message}");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
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

            var client = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001";
            client.BaseAddress = new Uri(apiBaseUrl);

            var registerData = new
            {
                username = model.Username,
                password = model.Password,
                email = model.Email,
                fullName = model.FullName
            };
            var content = new StringContent(JsonSerializer.Serialize(registerData), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("/api/auth/register", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RegisterResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode && result?.Success == true)
                {
                    TempData["Success"] = "Registration successful! Please login.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", result?.Message ?? "Registration failed");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error connecting to API: {ex.Message}");
            }

            return View(model);
        }
    }

    // ViewModels
    public class LoginViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    // Response models
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public UserInfo? User { get; set; }
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}