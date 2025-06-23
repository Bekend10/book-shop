using book_shop.Dto;

namespace book_shop.Services.Interfaces
{
    public interface IOrderDetailService
    {
        Task<object> GetAllOrderDetailsAsync();
        Task<object> CreateOrderDetailAsync(OrderDetailDto orderDetail);
        Task<object> UpdateOrderDetailAsync(int order_id , UpdateOrderDetailDto orderDetail);
        Task<object> DeleteOrderDetailAsync(int id);
    }
}
