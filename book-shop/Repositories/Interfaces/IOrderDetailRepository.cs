using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<object> GetOrderDetailsByOrderIdAsync(int orderId);
    }
}
