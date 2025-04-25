using book_shop.Dto;
using book_shop.EmailService;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace book_shop.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJWTService _jwtService;
        private readonly ILogger<AccountService> _logger;
        private readonly IEmailService _emailService;


        public AccountService(IAccountRepository accountRepository, IJWTService jwtService , ILogger<AccountService> logger , IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<object> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _accountRepository.GetUserByEmailAsync(registerDto.email);
                if (existingUser != null)
                    throw new Exception("Email đã tồn tại !");

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.password);

                var user = new User
                {
                    first_name = registerDto.first_name,
                    last_name = registerDto.last_name,
                    email = registerDto.email,
                    created_at = DateTime.UtcNow,
                };
                var full_name = registerDto.first_name + " " + registerDto.last_name;
                await _emailService.SendWelcomeEmailAsync(registerDto.email, full_name);

                var refresh_token = _jwtService.GenerateRefreshToken();

                var account = new Account
                {
                    email = registerDto.email,
                    role_id = 1,
                    password = passwordHash,
                    refresh_token = refresh_token,
                    refresh_token_ext = DateTime.UtcNow,
                    is_active = true,
                    is_verify = 1,
                };

                await _accountRepository.AddUserWithAccountAsync(user, account);
                _logger.LogInformation("Đăng nhập thành công email {email}" , account.email);
                return new
                {
                    status = HttpStatusCode.Created,
                    msg = "Đăng ký thành công !",
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex , "Lỗi khi đăng ký cho tài khoản email : {Email}" , registerDto.email);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message
                };
            }
        }

        public async Task<object> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _accountRepository.GetUserByEmailAsync(loginDto.email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.password, user.Account.password))
                {
                    _logger.LogWarning("Đăng nhập thất bại cho email: {Email}", loginDto.email);
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        msg = "Sai tài khoản hoặc mật khẩu chưa chính xác !"
                    };
                }

                var token = _jwtService.GenerateJwtToken(user);

                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đăng nhập thành công !",
                    access_token = token
                };
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập với email: {Email}", loginDto.email);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message
                };
            }
        }

        public async Task<object> LockAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if(account == null)
            {
                return new {
                    status = HttpStatusCode.NotFound ,
                    msg = "Không tìm thấy người dùng !"
                
                };
            }

            account.is_active = !account.is_active;
            await _accountRepository.UpdateAsync(account);

            if(account.is_active)
            {
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Mở khoá thành công !"
                };
            }
            return new
            {
                status = HttpStatusCode.OK,
                msg = "Khoá thành công !"
            };
        }      
    }
}