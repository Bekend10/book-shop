using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Services.Interfaces
{
    public interface ICartService
    {
        Task<object> AddToCartAsync(AddToCartDto dto);
        Task<object> RemoveFromCartAsync(int bookId);
        Task<object> ClearCartAsync();
        Task<List<CartDetailDto>> GetCartDetailsAsync();
        Task<object> UpdateCartItemQuantityAsync(UpdateQuantityToCartDto dto);
        Task<object> CheckoutAsync();
        Task<object> IsBookInCartAsync(int bookId);
        Task<int> GetTotalItemsInCartAsync();
        Task<decimal> GetTotalPriceInCartAsync();
        Task<object> AddCartDetail();
    }
}
