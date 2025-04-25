using book_shop.Dto;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace book_shop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            var user = await _accountService.RegisterAsync(registerDto);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _accountService.LoginAsync(loginDto);

            if (((dynamic)result).status == HttpStatusCode.OK)
            {
                var token = ((dynamic)result).access_token;

                Response.Cookies.Append("access_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");

            return Ok(new
            {
                status = HttpStatusCode.OK,
                msg = "Đăng xuất thành công!"
            });
        }

    }
}
