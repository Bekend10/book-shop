using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetOrderByUserId(int userId);
        Task<List<Order>> GetOrderByDate(DateTime date);
        Task<List<Order>> GetOrderByStatus(Enums.OrderEnumStatus.OrderStatus statusId);
        Task<double> GetAverangeOrderValue(DateTime? startDate, DateTime? endDate);
        Task<int> GetTotalProductSold(DateTime? startDate, DateTime? endDate);
        Task<List<ListOrderDto>> ListOrderDto(DateTime? startDate , DateTime? endDate);
    }
}
