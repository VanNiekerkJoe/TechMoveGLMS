using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TechMoveGLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Simple in-memory user database (in production, use a real database)
        private static readonly List<User> _users = new()
        {
            new User { Id = 1, Username = "admin", Password = "admin123", Role = "Administrator", Email = "admin@techmove.com", FullName = "System Administrator" },
            new User { Id = 2, Username = "manager", Password = "manager123", Role = "Manager", Email = "manager@techmove.com", FullName = "Operations Manager" },
            new User { Id = 3, Username = "user", Password = "user123", Role = "User", Email = "user@techmove.com", FullName = "Regular User" }
        };

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Find user by username
            var user = _users.FirstOrDefault(u => u.Username == request.Username);

            // Check if user exists and password matches
            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { success = false, message = "Invalid username or password" });
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Return user info (without password)
            return Ok(new
            {
                success = true,
                message = "Login successful",
                token = token,
                user = new
                {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.Role
                }
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // Check if username already exists
            if (_users.Any(u => u.Username == request.Username))
            {
                return BadRequest(new { success = false, message = "Username already exists" });
            }

            // Create new user
            var newUser = new User
            {
                Id = _users.Max(u => u.Id) + 1,
                Username = request.Username,
                Password = request.Password,
                Role = "User",
                Email = request.Email,
                FullName = request.FullName
            };

            _users.Add(newUser);

            return Ok(new { success = true, message = "User registered successfully" });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var username = User.Identity?.Name;
            var user = _users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            return Ok(new
            {
                success = true,
                user = new
                {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.Role
                }
            });
        }

        [HttpGet("users")]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetAllUsers()
        {
            var users = _users.Select(u => new
            {
                u.Id,
                u.Username,
                u.FullName,
                u.Email,
                u.Role
            });

            return Ok(users);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "TechMoveGLMS_SuperSecretKey_2024_ForJWT_Authentication_32Bytes"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName ?? user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "TechMoveGLMS_API",
                audience: _configuration["Jwt:Audience"] ?? "TechMoveGLMS_MVC",
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Model classes
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }
}