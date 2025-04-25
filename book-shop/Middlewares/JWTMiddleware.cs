using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace book_shop.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;
        private readonly IConfiguration _configuration;
        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger , IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(securityKey.ToString());

                    var principal = ValidateToken(token, key);
                    if (principal != null)
                    {
                        httpContext.User = principal;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Token validation failed: " + ex.Message);
                }
            }

            await _next(httpContext);
        }
        private ClaimsPrincipal ValidateToken(string token, byte[] key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = "BookShopAPI",
                    ValidAudience = "BookShopUser"
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError("JWT validation error: " + ex.Message);
                return null;
            }
        }
    }
}
