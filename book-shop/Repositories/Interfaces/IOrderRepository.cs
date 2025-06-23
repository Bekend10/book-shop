using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetOrderByUserId(int userId);
        Task<List<Order>> GetOrderByDate(DateTime date);
        Task<List<Order>> GetOrderByStatus(Enums.OrderEnumStatus.OrderStatus statusId);
    }
}
