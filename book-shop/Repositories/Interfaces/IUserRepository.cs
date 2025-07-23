using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<int> GetCurrentUserIdAsync();
        Task<User> GetByGoogleIdAsync(string googleId);
        Task<User> CreateNewUser(UserGoogleDto model);

    }
}
