using book_shop.Helpers.UserHelper;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class CartDetailService : ICartDetailService
    {
        private readonly ICartDetailRepository _cartDetailRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IBookRepository _bookRepository;
        private readonly UserHelper _userHelper;
        private readonly ILogger<CartDetailService> _logger;

        public CartDetailService(ICartDetailRepository cartDetailRepository, IBookRepository bookRepository, ILogger<CartDetailService> logger, ICartRepository cartRepository, UserHelper userHelper)
        {
            _cartDetailRepository = cartDetailRepository;
            _bookRepository = bookRepository;
            _logger = logger;
            _cartRepository = cartRepository;
            _userHelper = userHelper;
        }

        public async Task<object> GetCartDetailById(int cart_id)
        {
            try
            {
                var cart = await _cartDetailRepository.GetByIdAsync(cart_id);
                if (cart == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Giỏ hàng không tồn tại"
                    };
                }

                var cartDetails = await _cartDetailRepository.GetByIdAsync(cart.cart_detail_id);
                if (cartDetails == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Chi tiết giỏ hàng không tồn tại"
                    };
                }

                var book = await _bookRepository.GetByIdAsync(cartDetails.book_id);

                _logger.LogInformation("Lấy chi tiết giỏ hàng thành công");
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy chi tiết giỏ hàng thành công",
                    data = new
                    {
                        cartDetailId = cartDetails.cart_detail_id,
                        bookId = cartDetails.book_id,
                        quantity = cartDetails.quantity,
                        price = cartDetails.unit_price,
                        bookName = book.title,
                        bookImage = book.image_url,
                    }
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy giỏ hàng chi tiết !");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi lấy chi tiết giỏ hàng " + ex.Message
                };
            }
        }      
    }
}
