using book_shop.Models;

namespace book_shop.Services.Interfaces
{
    public interface IVnpayService
    {
        Task<string> CreatePaymentUrl(VnpayPaymentRequest model , HttpContext context);
        Task<(bool isValid, string status)> ValidateReturnUrl(IQueryCollection query);
    }
}
