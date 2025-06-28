using Microsoft.AspNetCore.Mvc;
using payment_service.Models;
using payment_service.Services.Interfaces;

namespace payment_service.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnpayService _vnpayService;

        public PaymentController(IVnpayService vnpayService)
        {
            _vnpayService = vnpayService;
        }

        [HttpPost("create-by-vnpay")]
        public IActionResult Create([FromBody] VnpayPaymentRequest model)
        {
            var context = HttpContext;
            var paymentUrl = _vnpayService.CreatePaymentUrl(model , context);
            return Ok(new { paymentUrl });
        }

        [HttpGet("vnpay-return")]
        public IActionResult VnpayReturn()
        {
            if (_vnpayService.ValidateReturnUrl(Request.Query, out string status))
            {
                return Ok(new { status });
            }

            return BadRequest("Invalid signature");
        }
    }
}
