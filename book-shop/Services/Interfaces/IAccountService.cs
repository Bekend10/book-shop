using book_shop.Dto;

namespace book_shop.Services.Interfaces
{
    public interface IAccountService
    {
        Task<object> RegisterAsync(RegisterDto registerDto);
        Task<object> LoginAsync(LoginDto loginDto);
        Task<object> LockAsync(int id);

    }
}
