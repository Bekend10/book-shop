using book_shop.Dto;
using book_shop.Enums;
using book_shop.Helpers.UserHelper;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly IVnpayService _vnpayService;
        private readonly UserHelper _userHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(IPaymentRepository paymentRepository, ILogger<PaymentService> logger, IOrderRepository orderRepository, UserHelper userHelper, IVnpayService vnpayService, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
            _orderRepository = orderRepository;
            _userHelper = userHelper;
            _vnpayService = vnpayService;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<object> CreatePayment(CreatePaymentDto model)
        {
            try
            {
                var isOrderExisting = await _orderRepository.GetByIdAsync(model.OrderId);
                if (isOrderExisting == null)
                {
                    _logger.LogError("Đơn hàng không tồn tại.");
                    return new { Success = false, Message = "Không tìm thấy đơn hàng" };
                }

                var newPayment = new Payment
                {
                    order_id = isOrderExisting.order_id,
                    payment_date = model.PaymentDate,
                    amount = isOrderExisting.total_amount,
                    payment_status = PaymentEnumStatus.Pending
                };

                var paymentModel = new VnpayPaymentRequest
                {
                    Amount = isOrderExisting.total_amount,
                    OrderId = isOrderExisting.order_id.ToString(),
                    OrderInfo = "Done",
                };


                switch (model.MethodId)
                {
                    case 1: //COD
                        newPayment.method_id = model.MethodId;
                        newPayment.payment_status = PaymentEnumStatus.Completed;
                        isOrderExisting.status = OrderEnumStatus.OrderStatus.Delivered;
                        await _paymentRepository.AddAsync(newPayment);
                        await _orderRepository.UpdateAsync(isOrderExisting);
                        break;
                    case 2: //VNPay
                        var httpContext = _httpContextAccessor.HttpContext;
                        if (httpContext == null)
                        {
                            _logger.LogError("HttpContext is null. Cannot proceed with payment creation.");
                            return new { Success = false, Message = "Internal server error. Please try again later." };
                        }

                        var vnpayResult = await _vnpayService.CreatePaymentUrl(paymentModel, httpContext);
                        if (vnpayResult == null)
                        {
                            newPayment.method_id = model.MethodId;
                            newPayment.payment_status = PaymentEnumStatus.Failed;
                            await _paymentRepository.AddAsync(newPayment);

                            _logger.LogError("Lỗi khi tạo thanh toán VNPay.");
                            return new { Success = false, Message = "Lỗi khi tạo thanh toán VNPay." };
                        }
                        else
                        {
                            newPayment.method_id = model.MethodId;
                            newPayment.payment_status = PaymentEnumStatus.Pending;
                            await _paymentRepository.AddAsync(newPayment);

                            isOrderExisting.status = OrderEnumStatus.OrderStatus.Processing;
                            await _orderRepository.UpdateAsync(isOrderExisting);

                            _logger.LogInformation("Đã khởi tạo thanh toán VNPay cho đơn hàng {OrderId}", model.OrderId);

                            return new
                            {
                                paymentUrl = vnpayResult,
                                status = HttpStatusCode.OK,
                                message = "Chuyển hướng đến thanh toán VNPAY",
                            };
                        }

                }
                return new
                {
                    message = "Thanh toán đã được tạo thành công.",
                    status = HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Loi khi tạo thanh toán: {Message}", ex.Message);
                return new
                {
                    message = "Lỗi khi tạo thanh toán. Vui lòng thử lại sau.",
                    status = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<object> GetAllPayments()
        {
            var payments = await _paymentRepository.GetAllPaymentsAsync();
            if (payments == null || !payments.Any())
            {
                _logger.LogInformation("Không có thanh toán nào được tìm thấy.");
                return new { status = HttpStatusCode.NotFound, message = "Không có thanh toán nào được tìm thấy." };
            }
            return new
            {
                status = HttpStatusCode.OK,
                message = "Danh sách thanh toán đã được lấy thành công.",
                data = payments
            };

        }

        public async Task<object> GetPaymentById(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                _logger.LogInformation("Không tìm thấy thanh toán với ID {PaymentId}", id);
                return new { status = HttpStatusCode.NotFound, message = "Không tìm thấy thanh toán." };
            }
            return new
            {
                status = HttpStatusCode.OK,
                message = "Thanh toán đã được tìm thấy.",
                data = payment
            };
        }

        public async Task<object> GetPaymentByMethodId(int id)
        {
            var payment = await _paymentRepository.GetPaymentByMethod(id);
            if (payment == null)
            {
                _logger.LogInformation("Không tìm thấy thanh toán với phương thức ID {MethodId}", id);
                return new { status = HttpStatusCode.NotFound, message = "Không tìm thấy thanh toán." };
            }
            return new
            {
                status = HttpStatusCode.OK,
                message = "Thanh toán đã được tìm thấy.",
                data = payment
            };
        }
    }
}
