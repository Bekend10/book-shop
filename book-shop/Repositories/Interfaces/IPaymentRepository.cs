using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment> GetPaymentByMethod(int Id);
        Task<Payment> GetByOrderId(int id);
        Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync();
    }
}
