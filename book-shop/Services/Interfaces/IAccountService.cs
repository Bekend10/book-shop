﻿using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Services.Interfaces
{
    public interface IAccountService
    {
        Task<object> RegisterAsync(RegisterDto registerDto);
        Task<object> LoginAsync(LoginDto loginDto);
        Task<object> LockAsync(int id);
        Task<object> RefreshTokenAsync(string refreshToken);
        Task<object> ForgotPasswordAsync(string email);
        Task<object> ChangePassword(ChangePasswordDto dto);
        string GenerateRandomPassword(int length = 10);
        Task<object> GetAllAccounts();
        Task<object> GetAccountById(int id);
        Task<object> DeleteAccountAysnc(int id);
        Task<object> VerifyEmailAsync(string email);
        Task<object> CreateNewAccountByAdmin(CreateUserByAdmin model);
        Task<object> UpdateAccountByAdmin(int id , UpdateUserByAdmin model);
        Task<(string token, User user)> LoginWithGoogleAsync(GoogleLoginDto dto);
        Task<(string token, User user)> LoginWithFacebookAsync(string accessToken);
    }
}
