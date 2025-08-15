using book_shop.Commons.CacheKey;
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
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<CartService> _logger;
        private readonly UserHelper _userHelper;
        private readonly IRedisCacheService _cacheService;

        public CartService(ICartRepository cartRepository, ILogger<CartService> logger, IBookRepository bookRepository, UserHelper userHelper, ICartDetailRepository cartDetailRepository, IAuthorRepository authorRepository, IRedisCacheService cacheService)
        {
            _cartRepository = cartRepository;
            _logger = logger;
            _bookRepository = bookRepository;
            _userHelper = userHelper;
            _cartDetailRepository = cartDetailRepository;
            _authorRepository = authorRepository;
            _cacheService = cacheService;
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
                        total_amount = book.price * dto.quantity,
                        created_at = DateTime.UtcNow,
                    };
                    await _cartRepository.AddAsync(Newcart);

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
                await _cacheService.RemoveAsync(CartCacheKeys.myCart(userId));

                return new { status = HttpStatusCode.OK, msg = "Thêm sách vào giỏ hàng thành công !" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sách vào giỏ hàng");
                return new { status = HttpStatusCode.InternalServerError, msg = "Đã xảy ra lỗi khi thêm sách vào giỏ hàng" };
            }
        }

        public async Task<object> ClearCartAsync()
        {
            try
            {
                var userId = _userHelper.GetCurrentUserId();
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null) return new { status = HttpStatusCode.NotFound, msg = "Giỏ hàng không tồn tại" };

                await _cartDetailRepository.DeleteAsync(cart.cart_id);
                await _cartRepository.DeleteAsync(cart.cart_id);

                await _cacheService.RemoveAsync(CartCacheKeys.myCart(userId));

                return new { status = HttpStatusCode.OK, msg = "Xóa giỏ hàng thành công !" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa giỏ hàng");
                return new { status = HttpStatusCode.InternalServerError, msg = "Đã xảy ra lỗi khi xóa giỏ hàng" };
            }
        }

        public async Task<object> GetCartDetailsAsync()
        {
            try
            {
                var userId = _userHelper.GetCurrentUserId();
                if (userId == null)
                {
                    return new
                    {
                        status = HttpStatusCode.Unauthorized,
                        msg = "Bạn cần đăng nhập để xem giỏ hàng"
                    };
                }

                var cached = await _cacheService.GetAsync<CartDto>(CartCacheKeys.myCart(userId));
                if (cached != null)
                {
                    return new
                    {
                        status = HttpStatusCode.OK,
                        msg = "Lấy chi tiết giỏ hàng thành công từ cache",
                        data = cached
                    };
                }

                    var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Giỏ hàng không tồn tại"
                    };
                }

                var cartDetails = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);
                if (cartDetails.Count == 0)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Giỏ hàng của bạn hiện đang trống"
                    };
                }
                var cartDto = new CartDto
                {
                    cart_id = cart.cart_id,
                    user_id = cart.user_id,
                    total_amount = cart.total_amount,
                    created_at = cart.created_at,
                    updated_at = cart.updated_at,
                    items = cartDetails.Select(cd => new CartDetailItemDto
                    {
                        cart_detail_id = cd.cart_detail_id,
                        book_id = cd.book_id,
                        quantity = cd.quantity,
                        book = new BookResponse
                        {
                            book_id = cd.book.book_id,
                            title = cd.book.title,
                            price = cd.book.price,
                            price_origin = cd.book.price_origin,
                            image_url = cd.book.image_url,
                            quantity = cd.book.quantity,
                            publisher = cd.book.publisher,
                            author = _authorRepository.GetByIdAsync(cd.book.author_id).Result?.name ?? "Không rõ tác giả"
                        }
                    }).ToList()
                };

                // Lưu vào cache
                await _cacheService.SetAsync(CartCacheKeys.myCart(userId), cartDto, TimeSpan.FromDays(1));

                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy chi tiết giỏ hàng thành công !",
                    data = cartDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết giỏ hàng");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Đã xảy ra lỗi khi lấy chi tiết giỏ hàng"
                };
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
                var userId = _userHelper.GetCurrentUserId();
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null)
                    return new { status = HttpStatusCode.NotFound, msg = "Giỏ hàng không tồn tại" };

                var detail = await _cartDetailRepository.GetByCartIdAndBookIdAsync(cart.cart_id, bookId);
                if (detail == null)
                    return new { status = HttpStatusCode.NotFound, msg = "Sách không có trong giỏ hàng" };

                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book == null)
                    return new { status = HttpStatusCode.NotFound, msg = "Sách không tồn tại" };

                await _cartDetailRepository.DeleteAsync(detail.cart_detail_id);

                var details = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);
                if (details.Count == 0)
                {
                    await _cartRepository.DeleteAsync(cart.cart_id);
                    return new { status = HttpStatusCode.OK, msg = "Giỏ hàng đã được xoá hoàn toàn" };
                }

                cart.total_amount = details.Sum(cd => cd.unit_price * cd.quantity);
                var newCart = new Cart
                {
                    user_id = cart.user_id,
                    total_amount = cart.total_amount,
                    created_at = cart.created_at,
                    updated_at = DateTime.Now
                };
                await _cartRepository.UpdateAsync(newCart);

                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Sách đã được xoá khỏi giỏ hàng"
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
                if (book == null)
                    return new { status = HttpStatusCode.BadRequest, msg = "Sách không tồn tại" };

                if (dto.quantity > book.quantity)
                    return new { status = HttpStatusCode.BadRequest, msg = "Số lượng vượt quá tồn kho" };

                detail.quantity = dto.quantity;
                await _cartDetailRepository.UpdateAsync(detail);

                var details = await _cartDetailRepository.GetByCartIdAsync(cart.cart_id);
                cart.total_amount = details.Sum(cd => cd.unit_price * cd.quantity);
                cart.updated_at = DateTime.Now;
                var newCart = new Cart
                {
                    user_id = cart.user_id,
                    total_amount = cart.total_amount,
                    created_at = cart.created_at,
                    updated_at = cart.updated_at
                };
                await _cartRepository.UpdateAsync(newCart);
                await _cacheService.RemoveAsync(CartCacheKeys.myCart(userId));
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
                items = cartDetails.Select(cd => new CartDetailItemDto
                {
                    cart_detail_id = cd.cart_detail_id,
                    book_id = cd.book_id,
                    quantity = cd.quantity,
                    book = new BookResponse
                    {
                        book_id = cd.book.book_id,
                        title = cd.book.title,
                        price = cd.book.price,
                        price_origin = cd.book.price_origin,
                        image_url = cd.book.image_url,
                        quantity = cd.book.quantity,
                        publisher = cd.book.publisher,
                    }
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
