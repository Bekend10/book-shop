using book_shop.Dto;
using book_shop.Enums;
using book_shop.Helpers.UserHelper;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBookDetailRepository _bookDetailRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ICartDetailRepository _cartDetailRepository;
        private readonly IBookRepository _book;
        private readonly UserHelper _userHelper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ILogger<OrderService> logger, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IBookRepository book, UserHelper userHelper, ICartRepository cartRepository, ICartDetailRepository cartDetailRepository, IUserRepository userRepository, IAddressRepository addressRepository = null, IBookDetailRepository bookDetailRepository = null, IAuthorRepository authorRepository = null)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _book = book;
            _userHelper = userHelper;
            _cartRepository = cartRepository;
            _cartDetailRepository = cartDetailRepository;
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _bookDetailRepository = bookDetailRepository;
            _authorRepository = authorRepository;
        }


        public async Task<object> CreateOrderAsync(OrderDto order)
        {
            try
            {
                var userId = _userHelper.GetCurrentUserId();
                if (userId <= 0)
                {
                    _logger.LogWarning("Không tìm thấy người dùng hiện tại.");
                    return new
                    {
                        status = HttpStatusCode.Unauthorized,
                        message = "Bạn cần đăng nhập để tạo đơn hàng."
                    };
                }

                var newOrder = new Order
                {
                    user_id = userId,
                    method_id = order.method_id,
                    order_date = DateTime.Now,
                    status = OrderEnumStatus.OrderStatus.Pending,
                    total_amount = 0
                };

                await _orderRepository.AddAsync(newOrder);
                _logger.LogInformation("Đơn hàng mới đã được tạo với ID: {OrderId}", newOrder.order_id);

                #region validate order details
                var isExistingBook = await _book.GetByIdAsync(order.book_id);
                if (isExistingBook == null)
                {
                    _logger.LogWarning("Sách với ID {BookId} không tồn tại.", order.book_id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Sách không tồn tại."
                    };
                }

                if (isExistingBook.quantity < 0)
                {
                    _logger.LogWarning("Số lượng sách không đủ để tạo đơn hàng.");
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        message = "Số lượng sách không đủ để tạo đơn hàng."
                    };
                }

                if (order.quantity <= 0)
                {
                    _logger.LogWarning("Số lượng đặt hàng phải lớn hơn 0.");
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        message = "Số lượng đặt hàng phải lớn hơn 0."
                    };
                }

                if (order.quantity > isExistingBook.quantity)
                {
                    _logger.LogWarning("Số lượng đặt hàng vượt quá số lượng sách hiện có.");
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        message = "Số lượng đặt hàng vượt quá số lượng sách hiện có."
                    };
                }

                #endregion

                var newOrderDetail = new OrderDetail
                {
                    order_id = newOrder.order_id,
                    book_id = order.book_id,
                    quantity = order.quantity,
                    unit_price = isExistingBook.price
                };

                newOrder.total_amount = newOrderDetail.quantity * newOrderDetail.unit_price;
                isExistingBook.quantity -= newOrderDetail.quantity;
                isExistingBook.is_bn = isExistingBook.quantity <= 0 ? 0 : 1; 

                await _book.UpdateAsync(isExistingBook);
                await _orderDetailRepository.AddAsync(newOrderDetail);
                await _orderRepository.UpdateAsync(newOrder);

                _logger.LogInformation("Đơn hàng đã được tạo thành công với ID: {OrderId}", newOrder.order_id);
                return new
                {
                    status = HttpStatusCode.Created,
                    message = "Đơn hàng đã được tạo thành công.",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi tạo đơn hàng: " + ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi tạo đơn hàng: " + ex.Message
                };
            }
        }

        public async Task<object> CreateOrderByCartAsync(OrderDto order)
        {
            try
            {
                var userId = _userHelper.GetCurrentUserId();
                if (userId <= 0)
                {
                    _logger.LogWarning("Không tìm thấy người dùng hiện tại.");
                    return new
                    {
                        status = HttpStatusCode.Unauthorized,
                        message = "Bạn cần đăng nhập để tạo đơn hàng."
                    };
                }

                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    _logger.LogWarning("Giỏ hàng không tồn tại cho người dùng với ID {UserId}.", userId);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Giỏ hàng không tồn tại."
                    };
                }

                var book = await _book.GetByIdAsync(cart.items.Select(x => x.book_id).First());
                if (book == null)
                {
                    _logger.LogWarning("Sách với ID {BookId} không tồn tại.", cart.book_id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Sách không tồn tại."
                    };
                }

                if (book.quantity <= 0)
                {
                    _logger.LogWarning("Số lượng sách không đủ để tạo đơn hàng.");
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        message = "Số lượng sách không đủ để tạo đơn hàng."
                    };
                }

                if (cart.quantity <= 0)
                {
                    _logger.LogWarning("Số lượng đặt hàng phải lớn hơn 0.");
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        message = "Số lượng đặt hàng phải lớn hơn 0."
                    };
                }

                if (cart.quantity > book.quantity)
                {
                    _logger.LogWarning("Số lượng đặt hàng vượt quá số lượng sách hiện có.");
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        message = "Số lượng đặt hàng vượt quá số lượng sách hiện có."
                    };
                }

                var newOrder = new Order
                {
                    user_id = userId,
                    method_id = order.method_id,
                    order_date = DateTime.Now,
                    status = OrderEnumStatus.OrderStatus.Pending,
                    total_amount = cart.quantity * book.price
                };

                await _orderRepository.AddAsync(newOrder);
                _logger.LogInformation("Đơn hàng mới đã được tạo với ID: {OrderId}", newOrder.order_id);

                var newOrderDetail = new OrderDetail
                {
                    order_id = newOrder.order_id,
                    book_id = book.book_id,
                    quantity = cart.quantity,
                    unit_price = book.price
                };

                book.quantity -= cart.quantity;
                await _book.UpdateAsync(book);
                await _orderDetailRepository.AddAsync(newOrderDetail);

                await _cartDetailRepository.DeleteAsync(cart.cart_id); // Hàm xóa toàn bộ chi tiết giỏ hàng theo giỏ
                await _cartRepository.DeleteAsync(cart.cart_id);

                _logger.LogInformation("Giỏ hàng và chi tiết giỏ hàng đã được xóa sau khi tạo đơn hàng với ID: {OrderId}", newOrder.order_id);

                return new
                {
                    status = HttpStatusCode.Created,
                    message = "Đơn hàng đã được tạo thành công."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng từ giỏ hàng.");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi tạo đơn hàng từ giỏ hàng: " + ex.Message
                };
            }
        }

        public async Task<object> DeleteOrderAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Đơn hàng với ID {OrderId} không tồn tại.", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Đơn hàng không tồn tại."
                    };
                }
                await _orderRepository.DeleteAsync(id);
                _logger.LogInformation("Đơn hàng với ID {OrderId} đã được xóa thành công.", id);

                var orderDetail = (OrderDetail)await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(id);
                if (orderDetail != null)
                {
                    await _orderDetailRepository.DeleteAsync(orderDetail.order_detail_id);
                    _logger.LogInformation("Chi tiết đơn hàng với ID {OrderDetailId} đã được xóa thành công.", orderDetail.order_detail_id);
                }
                _logger.LogInformation("Đơn hàng đã được xóa thành công với ID: {OrderId}", id);

                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Đơn hàng đã được xóa thành công."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa đơn hàng: " + ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi xóa đơn hàng: " + ex.Message
                };
            }
        }

        public async Task<object> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var orderResult = new List<OrderRespone>();

            foreach (var order in orders)
            {
                var user = await _userRepository.GetByIdAsync(order.user_id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", order.user_id);
                    continue;
                }
                var orderDetail = (OrderDetail)await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(order.order_id);
                var bookDetail = await _bookDetailRepository.GetBookDetailsByBookIdAsync(orderDetail?.book_id ?? 0);
                var book = await _book.GetByIdAsync(orderDetail?.book_id ?? 0);
                var author = await _authorRepository.GetByIdAsync(book?.author_id ?? 0);
                orderResult.Add(new OrderRespone
                {
                    order_id = order.order_id,
                    user = user,
                    orderDetail = orderDetail,
                    address = _addressRepository != null ? await _addressRepository.GetByIdAsync(user.address_id) : null,
                    order_date = order.order_date,
                    status = order.status,
                    total_amount = order.total_amount,
                    items = new BookResponseDto
                    {
                        book_id = orderDetail != null ? orderDetail?.book_id ?? 0 : 0,
                        quantity = orderDetail != null ? orderDetail?.quantity ?? 0 : 0,
                        image_url = bookDetail != null ? bookDetail?.book.image_url ?? null : null,
                        title = bookDetail != null ? bookDetail?.book.title : null,
                        price = bookDetail != null ? bookDetail.price : 0,
                        author = author != null ? new Author
                        {
                            author_id = author.author_id,
                            name = author.name,
                            bio = author.bio,
                        } : null
                    }
                });;
            }

            if (orders == null || !orders.Any())
            {
                _logger.LogInformation("Không có đơn hàng nào được tìm thấy.");
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Không có đơn hàng nào được tìm thấy."
                };
            }
            _logger.LogInformation("Lấy danh sách đơn hàng thành công.");
            return new
            {
                status = HttpStatusCode.OK,
                data = orderResult
            };
        }

        public async Task<object> GetMyOrdersAsync()
        {
            var userId = _userHelper.GetCurrentUserId();
            if (userId <= 0)
            {
                _logger.LogWarning("Không tìm thấy người dùng hiện tại.");
                return new
                {
                    status = HttpStatusCode.Unauthorized,
                    message = "Bạn cần đăng nhập để xem đơn hàng của mình."
                };
            }
            var orders = await _orderRepository.GetOrderByUserId(userId);
            if (orders == null)
            {
                _logger.LogInformation("Không có đơn hàng nào được tìm thấy cho người dùng với ID {UserId}.", userId);
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Không có đơn hàng nào được tìm thấy cho người dùng này."
                };
            }
            _logger.LogInformation("Lấy danh sách đơn hàng cho người dùng với ID {UserId} thành công.", userId);
            return new
            {
                status = HttpStatusCode.OK,
                message = "Lấy danh sách đơn hàng thành công.",
                data = orders
            };
        }

        public async Task<object> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Đơn hàng với ID {OrderId} không tồn tại.", id);
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Đơn hàng không tồn tại."
                };
            }
            _logger.LogInformation("Lấy đơn hàng với ID {OrderId} thành công.", id);
            return new
            {
                status = HttpStatusCode.OK,
                data = order
            };
        }

        public async Task<object> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = (OrderDetail)await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId);
            if (orderDetails == null)
            {
                _logger.LogWarning("Không tìm thấy chi tiết đơn hàng cho ID {OrderId}.", orderId);
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Không tìm thấy chi tiết đơn hàng."
                };
            }
            var book = await _book.GetByIdAsync(orderDetails.book_id);
            orderDetails.book = book;
            _logger.LogInformation("Lấy chi tiết đơn hàng cho ID {OrderId} thành công.", orderId);
            return new
            {
                status = HttpStatusCode.OK,
                data = orderDetails
            };
        }

        public async Task<object> GetOrdersByDateAsync(DateTime date)
        {
            var orders = await _orderRepository.GetOrderByDate(date);
            if (orders == null)
            {
                _logger.LogInformation("Không tìm thấy đơn hàng nào cho ngày {Date}.", date);
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Không tìm thấy đơn hàng nào cho ngày này."
                };
            }
            _logger.LogInformation("Lấy danh sách đơn hàng cho ngày {Date} thành công.", date);
            return new
            {
                status = HttpStatusCode.OK,
                data = orders
            };
        }

        public async Task<object> GetOrdersByStatusAsync(Enums.OrderEnumStatus.OrderStatus statusId)
        {
            var orders = await _orderRepository.GetOrderByStatus(statusId);
            if (!orders.Any())
            {
                _logger.LogInformation("Không tìm thấy đơn hàng nào với trạng thái ID {StatusId}.", statusId);
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Không tìm thấy đơn hàng nào với trạng thái này."
                };
            }
            _logger.LogInformation("Lấy danh sách đơn hàng với trạng thái ID {StatusId} thành công.", statusId);
            return new
            {
                status = HttpStatusCode.OK,
                message = "Lấy danh sách đơn hàng thành công.",
                data = orders
            };
        }

        public async Task<object> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrderByUserId(userId);
            if (orders == null)
            {
                _logger.LogInformation("Không tìm thấy đơn hàng nào cho người dùng với ID {UserId}.", userId);
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Không tìm thấy đơn hàng nào cho người dùng này."
                };
            }
            _logger.LogInformation("Lấy danh sách đơn hàng cho người dùng với ID {UserId} thành công.", userId);
            return new
            {
                status = HttpStatusCode.OK,
                data = orders
            };
        }

        public async Task<object> UpdateOrderAsync(int id, UpdateOrderDto order)
        {
            try
            {
                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                {
                    _logger.LogWarning("Đơn hàng với ID {OrderId} không tồn tại.", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Đơn hàng không tồn tại."
                    };
                }
                existingOrder.status = order.status;
                await _orderRepository.UpdateAsync(existingOrder);
                _logger.LogInformation("Cập nhật đơn hàng với ID {OrderId} thành công.", id);
                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Cập nhật đơn hàng thành công.",
                    data = existingOrder
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật đơn hàng: " + ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi cập nhật đơn hàng: " + ex.Message
                };
            }
        }
    }
}
