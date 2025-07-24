using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<int> GetCurrentUserIdAsync();
        Task<User> GetByGoogleIdAsync(string googleId);
        Task<User> GetByFacebookIdAsync(string facebookId);
        Task<User> CreateNewUser(UserGoogleDto model);
        Task<User> CreateNewUser(UserFaceBookDto model);
    }
}
