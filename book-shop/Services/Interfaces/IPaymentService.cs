using book_shop.Dto;

namespace book_shop.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<object> CreatePayment(CreatePaymentDto model);
        Task<object> GetPaymentById(int id);
        Task<object> GetAllPayments();
        Task<object> GetPaymentByMethodId(int id);
    }
}
