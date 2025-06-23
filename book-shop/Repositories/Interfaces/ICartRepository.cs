using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<CartDetailDto> GetCartByUserIdAsync(int userId);
    }
}
