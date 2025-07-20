using book_shop.Dto;
using book_shop.Services.Implementations;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace book_shop.Controllers
{
    [ApiController]
    [Route("api/v1/accounts")]
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
                    Expires = DateTime.Now.AddHours(1)
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _accountService.RefreshTokenAsync(refreshToken);
            if (result is not null && result.GetType() == typeof(System.Object) && ((dynamic)result).status == 401)
            {
                return Unauthorized(new { message = ((dynamic)result).message });
            }

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var result = await _accountService.ChangePassword(dto);
            return Ok(result);
        }

        [HttpPut]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            var result = await _accountService.ForgotPasswordAsync(email);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        [Route("get-accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            var result = await _accountService.GetAllAccounts();
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        [Route("get-detail")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var result = await _accountService.GetAccountById(id);
            if(result == null) { return NotFound(); }
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles ="admin")]
        [Route("delete-account")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var result = await _accountService.DeleteAccountAysnc(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("verify-email")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            var result = await _accountService.VerifyEmailAsync(email);
            return Ok(result);
        }

        [HttpPut]
        [Route("lock-account")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> LockAccount(int id)
        {
            var result = await _accountService.LockAsync(id);
            return Ok(result);
        }
    }
}
