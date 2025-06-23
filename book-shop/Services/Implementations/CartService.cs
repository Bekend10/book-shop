using book_shop.Dto;
using book_shop.Helpers.UserHelper;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartDetailRepository _cartDetailRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<CartService> _logger;
        private readonly UserHelper _userHelper;

        public CartService(ICartRepository cartRepository, ILogger<CartService> logger, IBookRepository bookRepository, UserHelper userHelper, ICartDetailRepository cartDetailRepository)
        {
            _cartRepository = cartRepository;
            _logger = logger;
            _bookRepository = bookRepository;
            _userHelper = userHelper;
            _cartDetailRepository = cartDetailRepository;
        }

        public async Task<object> AddToCartAsync(AddToCartDto dto)
        {
            try
            {
                var book = await _bookRepository.GetByIdAsync(dto.book_id);
                if (book == null || dto.quantity <= 0 || dto.quantity > book.quantity)
                {
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        msg = book == null ? "Không tìm thấy sách !" : "Số lượng sách không hợp lệ hoặc vượt quá tồn kho !"
                    };
                }

                int userId = _userHelper.GetCurrentUserId();
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    var Newcart = new Cart
                    {
                        user_id = userId,
                        total_amount = 0,
                        created_at = DateTime.UtcNow,
                        updated_at = DateTime.UtcNow
                    };
                    await _cartRepository.AddAsync(Newcart); // cart_id sẽ được EF gán
                }

                // Lấy lại cart_id đảm bảo EF đã gán
                if (cart.cart_id == 0)
                {
                    cart = await _cartRepository.GetCartByUserIdAsync(userId);
                }

                //Tạo hoặc cập nhật CartDetail
                var cartDetail = await _cartDetailRepository.GetByCartIdAndBookIdAsync(cart.cart_id, dto.book_id);
                if (cartDetail != null)
                {
                    cartDetail.quantity += dto.quantity;
                    await _cartDetailRepository.UpdateAsync(cartDetail);
                }
                else
                {
                    cartDetail = new CartDetail
                    {
                        cart_id = cart.cart_id,
                        book_id = dto.book_id,
                        quantity = dto.quantity,
                        unit_price = book.price
                    };
                    await _cartDetailRepository.AddAsync(cartDetail);
                }

                

                return new { status = HttpStatusCode.OK, msg = "Thêm sách vào giỏ hàng thành công !" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sách vào giỏ hàng");
                return new { status = HttpStatusCode.InternalServerError, msg = "Đã xảy ra lỗi khi thêm sách vào giỏ hàng" };
            }
        }

        public async Task<object> AddCartDetail()
        {
            var newCartDetail = new CartDetail
            {
                cart_id = 11,
                book_id = 18,
                quantity = 1,
                unit_price = 1
            };

            await _cartDetailRepository.AddAsync(newCartDetail);
            return new { status = HttpStatusCode.OK, msg = "Thêm chi tiết giỏ hàng thành công !" };
        }

        public async Task<object> ClearCartAsync()
        {
            try
            {
                var userId = _userHelper.GetCurrentUserId();
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null) return new { status = HttpStatusCode.NotFound, msg = "Giỏ hàng không tồn tại" };

                var cartDetails = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);
                foreach (var item in cartDetails)
                {
                    var book = await _bookRepository.GetByIdAsync(item.book_id);
                    if (book != null)
                    {
                        book.quantity += item.quantity;
                        await _bookRepository.UpdateAsync(book);
                    }
                }
                await _cartDetailRepository.DeleteAsync(cart.cart_id);
                await _cartRepository.DeleteAsync(cart.cart_id);

                return new { status = HttpStatusCode.OK, msg = "Xóa giỏ hàng thành công !" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa giỏ hàng");
                return new { status = HttpStatusCode.InternalServerError, msg = "Đã xảy ra lỗi khi xóa giỏ hàng" };
            }
        }

        public async Task<List<CartDetailDto>> GetCartDetailsAsync()
        {
            try
            {
                var userId = _userHelper.GetCurrentUserId();
                if (userId == null) return new List<CartDetailDto>();

                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null) return new List<CartDetailDto>();

                var cartDetails = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);

                var cartDetailDtos = cartDetails.Select(cd => new CartDetailDto
                {
                    cart_detail_id = cd.cart_detail_id,
                    book_id = cd.book_id,
                    quantity = cd.quantity,
                    unit_price = cd.unit_price,
                }).ToList();

                return cartDetailDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết giỏ hàng");
                return new List<CartDetailDto>();
            }
        }


        public async Task<int> GetTotalItemsInCartAsync()
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(_userHelper.GetCurrentUserId());
                if (cart == null) return 0;
                var details = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);
                return details.Sum(cd => cd.quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính tổng sản phẩm");
                return 0;
            }
        }

        public async Task<decimal> GetTotalPriceInCartAsync()
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(_userHelper.GetCurrentUserId());
                if (cart == null) return 0;
                var details = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);
                return details.Sum(cd => cd.unit_price * cd.quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính tổng tiền");
                return 0;
            }
        }

        public async Task<object> IsBookInCartAsync(int bookId)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(_userHelper.GetCurrentUserId());
                var detail = cart != null ? await _cartDetailRepository.GetByCartIdAndBookIdAsync(cart.cart_id, bookId) : null;
                return detail != null
                    ? new { status = HttpStatusCode.OK, msg = "Sách có trong giỏ hàng !" }
                    : new { status = HttpStatusCode.NotFound, msg = "Sách không có trong giỏ hàng" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kiểm tra sách");
                return new { status = HttpStatusCode.InternalServerError, msg = "Lỗi khi kiểm tra sách trong giỏ hàng" };
            }
        }

        public async Task<object> RemoveFromCartAsync(int bookId)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(_userHelper.GetCurrentUserId());
                if (cart == null) return new { status = HttpStatusCode.NotFound, msg = "Giỏ hàng không tồn tại" };
                var detail = await _cartDetailRepository.GetByCartIdAndBookIdAsync(cart.cart_id, bookId);
                if (detail == null) return new { status = HttpStatusCode.NotFound, msg = "Sách không có trong giỏ hàng" };
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book == null) return new { status = HttpStatusCode.NotFound, msg = "Sách không tồn tại" };
                // Xoá giỏ hàng khi đặt hàng và xoá luôn giỏ hàng chi tiết
                await _cartDetailRepository.DeleteAsync(detail.cart_detail_id);
                var details = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);
                if (details.Count == 0)
                {
                    await _cartRepository.DeleteAsync(cart.cart_id);
                    return new { status = HttpStatusCode.OK, msg = "Giỏ hàng đã được xoá" };
                }

                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Sách đã được xoá khỏi giỏ hàng !",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa sách khỏi giỏ hàng");
                return new { status = HttpStatusCode.InternalServerError, msg = "Lỗi khi xóa sách khỏi giỏ hàng" };
            }
        }

        public async Task<object> UpdateCartItemQuantityAsync(UpdateQuantityToCartDto dto)
        {
            try
            {
                if (dto.quantity <= 0)
                    return new { status = HttpStatusCode.BadRequest, msg = "Số lượng không hợp lệ" };

                var userId = _userHelper.GetCurrentUserId();
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null) return new { status = HttpStatusCode.NotFound, msg = "Giỏ hàng không tồn tại" };

                var detail = await _cartDetailRepository.GetByCartIdAndBookIdAsync(cart.cart_id, dto.book_id);
                if (detail == null) return new { status = HttpStatusCode.NotFound, msg = "Sách không có trong giỏ hàng" };

                var book = await _bookRepository.GetByIdAsync(dto.book_id);
                if (book == null || dto.quantity > book.quantity)
                    return new { status = HttpStatusCode.BadRequest, msg = "Số lượng vượt quá tồn kho hoặc sách không tồn tại" };

                int diff = dto.quantity - detail.quantity;
                detail.quantity = dto.quantity;
                await _cartDetailRepository.UpdateAsync(detail);

                book.quantity -= diff;
                await _bookRepository.UpdateAsync(book);

                var details = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);
                cart.total_amount = details.Sum(cd => cd.unit_price * cd.quantity);
                Cart newCart = new Cart
                {
                    cart_id = cart.cart_id,
                    user_id = cart.user_id,
                    total_amount = cart.total_amount,
                    created_at = cart.created_at,
                    updated_at = DateTime.UtcNow
                };
                await _cartRepository.UpdateAsync(newCart);

                return new { status = HttpStatusCode.OK, msg = "Cập nhật thành công" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật số lượng sản phẩm");
                return new { status = HttpStatusCode.InternalServerError, msg = "Lỗi cập nhật số lượng sản phẩm" };
            }
        }

        public Task<object> CheckoutAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetCartByUserIdAsync(int userId)
        {
            if (userId == null)
            {
                return new { status = HttpStatusCode.Unauthorized, msg = "Bạn cần đăng nhập để xem giỏ hàng" };
            }
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return new { status = HttpStatusCode.NotFound, msg = "Giỏ hàng không tồn tại" };
            }
            var cartDetails = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);
            var cartDto = new CartDto
            {
                cart_id = cart.cart_id,
                user_id = cart.user_id,
                total_amount = cart.total_amount,
                created_at = cart.created_at,
                updated_at = cart.updated_at,
                items = cartDetails.Select(cd => new CartDetailDto
                {
                    cart_detail_id = cd.cart_detail_id,
                    book_id = cd.book_id,
                    quantity = cd.quantity,
                    unit_price = cd.unit_price,
                }).ToList()
            };
            return new
            {
                status = HttpStatusCode.OK,
                msg = "Lấy giỏ hàng thành công",
                data = cartDto
            };

        }
    }
}
