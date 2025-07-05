using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace book_shop.Controllers
{
    [Route("api/v1/payment-result")]
    public class PaymentResult : Controller
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentResult(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpGet("successfully")]
        public async Task<IActionResult> PaymentSuccess(int orderId)
        {
            var payment = await _paymentRepository.GetByOrderId(orderId);
            var result = new Payment
            {
                order_id = orderId,
                payment_id = payment.payment_id,
                method_id = payment.method_id,
                amount = payment.amount,
                payment_date = DateTime.Now,
            };
            return View("PaymentSuccess" , result);
        }

        [HttpGet("fail")]
        public async Task<IActionResult> PaymentFail(int orderId)
        {
            var payment = await _paymentRepository.GetByIdAsync(orderId);
            var result = new Payment
            {
                order_id = orderId,
                payment_id = payment.payment_id,
                method_id = payment.method_id,
                amount = payment.amount,
                payment_date = DateTime.Now,
            };
            return View("PaymentFail" , result);
        }
    }
}
