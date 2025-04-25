using book_shop.Models;
using book_shop.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace book_shop.Services.Implementations
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;
        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
            new Claim(ClaimTypes.Email, user.email),
            new Claim(ClaimTypes.Name, $"{user.first_name} {user.last_name}"),
            new Claim(ClaimTypes.Role , user.Account.role_id.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
        }
    }
}
