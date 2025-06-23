using book_shop.Dto;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace book_shop.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create-order")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto order)
        {
            var createdOrder = await _orderService.CreateOrderAsync(order);
            return Ok(createdOrder);
        }

        [HttpGet("get-all-orders")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("get-order-by-id")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            return Ok(order);
        }

        [HttpGet("get-orders-by-user-id")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet("get-my-orders")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _orderService.GetMyOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("get-orders-by-date")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByDate(DateTime date)
        {
            var orders = await _orderService.GetOrdersByDateAsync(date);
            return Ok(orders);
        }

        [HttpPut("update-order")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto order)
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(id, order);
            if (updatedOrder == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            return Ok(updatedOrder);
        }

        [HttpDelete("delete-order")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            return Ok(result);
        }

        [HttpGet("get-order-details-by-order-id")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetailsByOrderId(int orderId)
        {
            var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(orderId);
            if (orderDetails == null)
            {
                return NotFound(new { message = "Order details not found" });
            }
            return Ok(orderDetails);
        }

        [HttpGet("get-orders-by-status")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByStatus(Enums.OrderEnumStatus.OrderStatus statusId)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(statusId);
            return Ok(orders);
        }

        [HttpPost("create-order-by-cart")]
        [Authorize]
        public async Task<IActionResult> CreateOrderByCart([FromBody] OrderDto order)
        {
            var createdOrder = await _orderService.CreateOrderByCartAsync(order);
            return Ok(createdOrder);

        }
    }
}
