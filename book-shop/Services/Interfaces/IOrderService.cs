using book_shop.Dto;

namespace book_shop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<object> GetAllOrdersAsync();
        Task<object> GetOrderByIdAsync(int id);
        Task<object> GetOrdersByUserIdAsync(int userId);
        Task<object> GetMyOrdersAsync();
        Task<object> GetOrdersByDateAsync(DateTime date);
        Task<object> CreateOrderAsync(OrderDto order);
        Task<object> CreateOrderByCartAsync(OrderDto order);
        Task<object> UpdateOrderAsync(int id , UpdateOrderDto order);
        Task<object> DeleteOrderAsync(int id);
        Task<object> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<object> GetOrdersByStatusAsync(Enums.OrderEnumStatus.OrderStatus statusId);
    }
}
