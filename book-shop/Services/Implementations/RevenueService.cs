using book_shop.Enums;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class RevenueService : IRevenueService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookDetailRepository _bookDetailRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<RevenueService> _logger;

        public RevenueService(ICategoryRepository categoryRepository, IPaymentRepository paymentRepository, IOrderRepository orderRepository, IUserRepository userRepository, IBookDetailRepository bookDetailRepository, IBookRepository bookRepository, ILogger<RevenueService> logger)
        {
            _categoryRepository = categoryRepository;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _bookDetailRepository = bookDetailRepository;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        public async Task<object> GetRevenueAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var totalMonthRevenue = await _paymentRepository.GetMonthlyRevenue(startDate, endDate);
                var totalRevenue = await _paymentRepository.GetPaymentTotalByStatus();
                var totalOrder = await _orderRepository.GetTotalProductSold(startDate , endDate);
                var totalProductSold = await _orderRepository.GetTotalProductSold(startDate, endDate);
                var topProducts = await _bookRepository.GetTopProducts(startDate, endDate);
                var revenueByCategory = await _categoryRepository.GetRevenueByCategory(startDate, endDate);
                var listOrderDto = await _orderRepository.ListOrderDto(startDate, endDate);
                if (totalMonthRevenue == null || !totalMonthRevenue.Any())
                {
                    string msg;

                    if (startDate.HasValue && endDate.HasValue)
                        msg = "Không có doanh thu trong khoảng thời gian này.";
                    else if (startDate.HasValue)
                        msg = "Không có doanh thu từ ngày bắt đầu.";
                    else if (endDate.HasValue)
                        msg = "Không có doanh thu đến ngày kết thúc.";
                    else
                        msg = "Không có doanh thu.";

                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg
                    };
                }

                return new
                {
                    msg = "Lấy doanh thu thành công.",
                    status = HttpStatusCode.OK,
                    total_revenue = totalRevenue,
                    total_orders = totalOrder,
                    avg_order_value = totalRevenue / totalOrder,
                    total_product_sold = totalProductSold,
                    monthly_revenue = totalMonthRevenue,
                    top_products = topProducts,
                    revenue_by_category = revenueByCategory,
                    orders = listOrderDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy doanh thu");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi lấy doanh thu: " + ex.Message
                };
            }
        }
    }
}
