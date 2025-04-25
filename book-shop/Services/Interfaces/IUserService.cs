using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<object> CreateAsync(CreateUserDto dto);
        Task<object> UpdateAsync(int id, UpdateUserDto dto);
        Task<object> DeleteAsync(int id);
    }
}
