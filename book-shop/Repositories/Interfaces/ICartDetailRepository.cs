using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface ICartDetailRepository : IRepository<CartDetail>
    {
        Task<CartDetail> GetByCartIdAndBookIdAsync(int cartId, int bookId);
        Task<List<CartDetail>> GetByCartIdAsync(int cartId); 
        Task DeleteByCartIdAndBookIdAsync(int cartId , int bookId);
    }
}
