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
using System.Security.Cryptography;
using System.Text;

namespace book_shop.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;
        private readonly ILogger<AccountService> _logger;
        private readonly IEmailService _emailService;


        public AccountService(IAccountRepository accountRepository, IJWTService jwtService, ILogger<AccountService> logger, IEmailService emailService, IUserRepository userRepository)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<object> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _accountRepository.GetAccountByEmailAsync(registerDto.email);
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

                var refresh_token = _jwtService.GenerateRefreshToken();

                var account = new Account
                {
                    email = registerDto.email,
                    role_id = 1,
                    password = passwordHash,
                    refresh_token = refresh_token,
                    refresh_token_ext = DateTime.UtcNow,
                    is_active = true,
                };

                await _emailService.SendWelcomeEmailAsync(registerDto.email, full_name);

                await _accountRepository.AddUserWithAccountAsync(user, account);
                _logger.LogInformation("Đăng nhập thành công email {email}", account.email);
                return new
                {
                    status = HttpStatusCode.Created,
                    msg = "Đăng ký thành công !",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký cho tài khoản email : {Email}", registerDto.email);
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
                var account = await _accountRepository.GetAccountByEmailAsync(loginDto.email);
                if (account == null || !BCrypt.Net.BCrypt.Verify(loginDto.password, account.password))
                {
                    _logger.LogWarning("Đăng nhập thất bại cho email: {Email}", loginDto.email);
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        msg = "Sai tài khoản hoặc mật khẩu chưa chính xác !"
                    };
                }
                var accessToken = _jwtService.GenerateJwtToken(account);
                var refreshToken = _jwtService.GenerateRefreshToken();

                account.refresh_token = refreshToken;
                account.refresh_token_ext = DateTime.UtcNow.AddDays(7);
                await _accountRepository.UpdateAsync(account);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đăng nhập thành công !",
                    access_token = accessToken,
                    refresh_token = refreshToken
                };
            }
            catch (Exception ex)
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
            if (account == null)
            {
                return new
                {
                    status = HttpStatusCode.NotFound,
                    msg = "Không tìm thấy người dùng !"

                };
            }

            account.is_active = !account.is_active;
            await _accountRepository.UpdateAsync(account);

            if (account.is_active)
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

        public async Task<object> RefreshTokenAsync(string refreshToken)
        {
            var account = await _accountRepository.GetByRefreshTokenAsync(refreshToken);
            if (account == null || account.refresh_token_ext < DateTime.UtcNow)
            {
                return new { status = 401, message = "Refresh token không hợp lệ hoặc đã hết hạn." };
            }

            var newAccessToken = _jwtService.GenerateJwtToken(account);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            account.refresh_token = newRefreshToken;
            account.refresh_token_ext = DateTime.UtcNow.AddDays(7);
            await _accountRepository.UpdateAsync(account);

            return new { accessToken = newAccessToken, refreshToken = newRefreshToken };
        }

        public async Task<object> ForgotPasswordAsync(string email)
        {
            try
            {
                var existingAccount = await _accountRepository.GetAccountByEmailAsync(email);
                if (existingAccount == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy tài khoản !"
                    };
                }
                var newPassword = GenerateRandomPassword();
                await _emailService.SendResetPassword(email, email, newPassword);
                newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                existingAccount.password = newPassword;
                await _accountRepository.UpdateAsync(existingAccount);

                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Khôi phục mật khẩu thành công !"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi quên mật khẩu với email: {Email}", email);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message
                };
            }
        }

        public async Task<object> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                var existingAccount = await _accountRepository.GetByIdAsync(dto.account_id);
                if (existingAccount == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy tài khoản !"
                    };
                }

                var oldPass = existingAccount.password;
                var newPass = BCrypt.Net.BCrypt.HashPassword(dto.new_password);
                if (BCrypt.Net.BCrypt.Verify(dto.old_password, oldPass))
                {
                    if (BCrypt.Net.BCrypt.Verify(dto.old_password, newPass))
                    {
                        return new
                        {
                            status = HttpStatusCode.BadRequest,
                            msg = "Mật khẩu mới không được phép giống mật khẩu cũ !"
                        };
                    }

                    existingAccount.password = newPass;
                    await _accountRepository.UpdateAsync(existingAccount);
                    return new
                    {
                        status = HttpStatusCode.OK,
                        msg = "Đổi mật khẩu thành công !"
                    };

                }
                else
                {
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        msg = "Mật khẩu cũ không chính xác !"
                    };
                }

            }
            catch (Exception ex)
            {
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message
                };
            }
        }

        public string GenerateRandomPassword(int length = 10)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            StringBuilder password = new StringBuilder();
            byte[] randomBytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            foreach (byte b in randomBytes)
            {
                password.Append(validChars[b % validChars.Length]);
            }

            return password.ToString();
        }

        public async Task<object> GetAllAccounts()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return new
            {
                status = HttpStatusCode.OK,
                msg = "Lấy danh sách tài khoản thành công !",
                accounts
            };
        }

        public async Task<object> GetAccountById(int id)
        {
            try
            {
                var existingAccount = await _accountRepository.GetByIdAsync(id);
                if (existingAccount == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy tài khoản !"
                    };
                }
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy tài khoản thành công !",
                    account = existingAccount
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message
                };
            }
        }

        public async Task<object> DeleteAccountAysnc(int id)
        {
            try
            {
                var existingAccount = await _accountRepository.GetByIdAsync(id);
                if (existingAccount == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy tài khoản !"
                    };
                }
                await _accountRepository.DeleteAsync(id);
                await _userRepository.DeleteAsync(existingAccount.user_id);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Xoá tài khoản thành công !"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message
                };
            }
        }

        public async Task<object> VerifyEmailAsync(string email)
        {
            try
            {
                var existingAccount = await _accountRepository.GetAccountByEmailAsync(email);
                if (existingAccount == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy tài khoản !"
                    };
                }

                existingAccount.is_verify = 1;
                await _accountRepository.UpdateAsync(existingAccount);

                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Xác thực tài khoản thành công !"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message
                };
            }
        }
    }
}