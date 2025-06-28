using book_shop.Dto;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace book_shop.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IVnpayService _vnpayService;
        public PaymentController(IPaymentService paymentService, IVnpayService vnpayService)
        {
            _paymentService = paymentService;
            _vnpayService = vnpayService;
        }

        [HttpPost("create-payment")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var result = await _paymentService.CreatePayment(model);
            if (result is not null)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo thanh toán. Vui lòng thử lại sau." });
        }

        [HttpGet("return-vnpay")]
        [AllowAnonymous]
        public async Task<IActionResult> VnpayReturn()
        {
            var query = Request.Query;
            var (isValid, status) = await _vnpayService.ValidateReturnUrl(query);

            if (status == "Success")
            {
                return RedirectToAction("PaymentSuccess", "PaymentResult", new { orderId = query["vnp_TxnRef"] });
            }
            else
            {
                return RedirectToAction("PaymentFail", "PaymentResult", new { orderId = query["vnp_TxnRef"] });
            }
        }

        [HttpGet("get-all-payments")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPayments();
            if (payments != null)
            {
                return Ok(payments);
            }
            return NotFound(new { message = "Không tìm thấy thanh toán nào." });
        }

        [HttpGet("get-payment-by-id")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID không hợp lệ." });
            }

            var payment = await _paymentService.GetPaymentById(id);
            if (payment != null)
            {
                return Ok(payment);
            }
            return NotFound(new { message = "Không tìm thấy thanh toán với ID đã cho." });

        }

        [HttpGet("get-payment-by-method-id")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPaymentByMethodId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID không hợp lệ." });
            }

            var payment = await _paymentService.GetPaymentByMethodId(id);
            if (payment != null)
            {
                return Ok(payment);
            }
            return NotFound(new { message = "Không tìm thấy thanh toán với phương thức ID đã cho." });
        }
    }
}
