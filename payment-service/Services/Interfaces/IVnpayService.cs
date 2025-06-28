using payment_service.Models;

namespace payment_service.Services.Interfaces
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(VnpayPaymentRequest model, HttpContext context);
        bool ValidateReturnUrl(IQueryCollection query, out string status);
    }
}
