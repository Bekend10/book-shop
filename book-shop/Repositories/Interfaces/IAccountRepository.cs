using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetAccountByEmailAsync(string email);
        Task AddUserWithAccountAsync(User user, Account account);
        Task<Account> GetByRefreshTokenAsync(string refreshToken);
    }
}
