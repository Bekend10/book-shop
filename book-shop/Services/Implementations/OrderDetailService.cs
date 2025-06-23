using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ILogger<OrderDetailService> _logger;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, ILogger<OrderDetailService> logger)
        {
            _orderDetailRepository = orderDetailRepository;
            _logger = logger;
        }

        public async Task<object> GetAllOrderDetailsAsync()
        {
            var orderDetails = await _orderDetailRepository.GetAllAsync();
            if (orderDetails == null || !orderDetails.Any())
            {
                _logger.LogInformation("Không tìm thấy chi tiết đơn hàng nào.");
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Không tìm thấy chi tiết đơn hàng nào."
                };
            }
            _logger.LogInformation("Lấy danh sách chi tiết đơn hàng thành công.");
            return new
            {
                status = HttpStatusCode.OK,
                data = orderDetails
            };
        }

        public async Task<object> CreateOrderDetailAsync(OrderDetailDto orderDetail)
        {
            try
            {
                var newOrderDetail = new OrderDetail
                {
                    book_id = orderDetail.book_id,
                    quantity = orderDetail.quantity,
                    unit_price = orderDetail.unit_price
                };
                await _orderDetailRepository.AddAsync(newOrderDetail);
                _logger.LogInformation("Thêm chi tiết đơn hàng thành công với ID: {OrderDetailId}", newOrderDetail.order_detail_id);
                return new
                {
                    status = HttpStatusCode.Created,
                    message = "Thêm chi tiết đơn hàng thành công",
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Lỗi khi tạo chi tiết đơn hàng: " + ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi tạo chi tiết đơn hàng: " + ex.Message
                };
            }
        }

        public async Task<object> UpdateOrderDetailAsync(int order_id, UpdateOrderDetailDto orderDetail)
        {
            var existingOrderDetail = await _orderDetailRepository.GetByIdAsync(order_id);
            if (existingOrderDetail == null)
            {
                _logger.LogWarning("Chi tiết đơn hàng với ID {OrderId} không tồn tại.", order_id);
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Chi tiết đơn hàng không tồn tại."
                };
            }
            try
            {
                var updatedOrderDetail = new OrderDetail
                {
                    order_detail_id = order_id,
                    book_id = orderDetail.book_id,
                    quantity = orderDetail.quantity,
                    unit_price = orderDetail.unit_price
                };
                await _orderDetailRepository.UpdateAsync(updatedOrderDetail);
                _logger.LogInformation("Cập nhật chi tiết đơn hàng thành công với ID: {OrderId}", order_id);
                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Cập nhật chi tiết đơn hàng thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật chi tiết đơn hàng: " + ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi cập nhật chi tiết đơn hàng: " + ex.Message
                };
            }
        }

        public async Task<object> DeleteOrderDetailAsync(int id)
        {
            try
            {
                var orderDetail = await _orderDetailRepository.GetByIdAsync(id);
                if (orderDetail == null)
                {
                    _logger.LogWarning("Chi tiết đơn hàng với ID {OrderDetailId} không tồn tại.", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Chi tiết đơn hàng không tồn tại."
                    };
                }
                await _orderDetailRepository.DeleteAsync(id);
                _logger.LogInformation("Xoá chi tiết đơn hàng thành công với ID: {OrderDetailId}", id);
                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Xoá chi tiết đơn hàng thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa chi tiết đơn hàng: " + ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi xóa chi tiết đơn hàng: " + ex.Message
                };
            }
        }
    }
}
