using CodeSpace.Common.ApiDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Repos;
using Common.Entities;

namespace CodeSpace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _cfg;

        public AuthController(AppDbContext db, IConfiguration cfg)
        {
            _db = db;
            _cfg = cfg;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest req)
        {
            if (_db.Users.Any(u => u.Username == req.Username))
                return Conflict("Username taken");

            var user = new User
            {
                Username = req.Username,
                Password = req.Password,
                Email = req.Email,
                CreatedAt = DateTime.UtcNow,
                IsAdmin = false
            };
            _db.Users.Add(user);
            _db.SaveChanges();

            var token = GenerateJwtToken(user);

            return Ok(new LoginResponse(token, user.Username, user.IsAdmin, user.Id));
        }


        [HttpPost("login")]
        public IActionResult Login(LoginRequest req)
        {
            var user = _db.Users.SingleOrDefault(u => u.Username == req.Username && u.Password == req.Password);
            if (user == null) return Unauthorized();

            var token = GenerateJwtToken(user);
            return Ok(new LoginResponse(token, user.Username, user.IsAdmin, user.Id));
        }

        private string GenerateJwtToken(User user)
        {
            var key = _cfg["JwtKey"] ?? "dev-sample-key-change-me";
            var issuer = _cfg["JwtIssuer"] ?? "CodeSpaceApi";
            var audience = _cfg["JwtAudience"] ?? "CodeSpaceWeb";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("isAdmin", user.IsAdmin.ToString().ToLower())   // <-- add
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
