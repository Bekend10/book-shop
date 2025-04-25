using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
}
