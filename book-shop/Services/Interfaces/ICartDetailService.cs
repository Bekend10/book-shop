using book_shop.Models;

namespace book_shop.Services.Interfaces
{
    public interface ICartDetailService
    {
        Task<object> GetCartDetailById(int cart_id);
    }
}
