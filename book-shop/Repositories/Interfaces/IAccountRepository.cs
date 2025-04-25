using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserWithAccountAsync(User user, Account account);
    }
}
