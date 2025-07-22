using book_shop.Data;
using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order entity)
        {
            await _context.Orders.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var orders = await _context.Orders
                .Select(o => new Order
                {
                    order_id = o.order_id,
                    user_id = o.user_id,
                    order_date = o.order_date,
                    total_amount = o.total_amount,
                    method_id = o.method_id,
                    status = o.status
                }).ToListAsync();
            return orders;
        }

        public Task<double> GetAverangeOrderValue(DateTime? startDate, DateTime? endDate)
        {
            if(startDate.HasValue && endDate.HasValue)
            {
                return _context.Orders
                    .Where(o => o.order_date >= startDate.Value && o.order_date <= endDate.Value && o.status == Enums.OrderEnumStatus.OrderStatus.Delivered)
                    .AverageAsync(o => (double)o.total_amount);
            }
            else if (startDate.HasValue)
            {
                return _context.Orders
                    .Where(o => o.order_date >= startDate.Value && o.status == Enums.OrderEnumStatus.OrderStatus.Delivered)
                    .AverageAsync(o => (double)o.total_amount);
            }
            else if (endDate.HasValue)
            {
                return _context.Orders
                    .Where(o => o.order_date <= endDate.Value && o.status == Enums.OrderEnumStatus.OrderStatus.Delivered)
                    .AverageAsync(o => (double)o.total_amount);
            }
            else
            {
                return _context.Orders
                    .Where(o => o.status == Enums.OrderEnumStatus.OrderStatus.Delivered)
                    .AverageAsync(o => (double)o.total_amount);
            }
        }

        public Task<Order> GetByIdAsync(int id)
        {
            var order = _context.Orders
                .Select(o => new Order
                {
                    order_id = o.order_id,
                    user_id = o.user_id,
                    order_date = o.order_date,
                    method_id = o.method_id,
                    total_amount = o.total_amount,
                    status = o.status
                })
                .FirstOrDefaultAsync(o => o.order_id == id);
            return order;
        }

        public async Task<List<Order>> GetOrderByDate(DateTime date)
        {
            var orders = await _context.Orders
                .Where(x => x.order_date == date)
                .ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetOrderByStatus(Enums.OrderEnumStatus.OrderStatus statusId)
        {
            var orders = await _context.Orders
                .Where(x => x.status == statusId)
                .ToListAsync();

            return orders;
        }

        public async Task<List<Order>> GetOrderByUserId(int userId)
        {
            var orders = await _context.Orders
                .Where(x => x.user_id == userId)
                .ToListAsync();
            return orders;
        }

        public Task<int> GetTotalProductSold(DateTime? startDate, DateTime? endDate)
        {
            if(startDate.HasValue && endDate.HasValue)
            {
                return _context.Orders
                    .Where(o => o.order_date >= startDate.Value && o.order_date <= endDate.Value && o.status == Enums.OrderEnumStatus.OrderStatus.Delivered)
                    .CountAsync();
            }
            else if (startDate.HasValue)
            {
                return _context.Orders
                    .Where(o => o.order_date >= startDate.Value && o.status == Enums.OrderEnumStatus.OrderStatus.Delivered)
                    .CountAsync();
            }
            else if (endDate.HasValue)
            {
                return _context.Orders
                    .Where(o => o.order_date <= endDate.Value && o.status == Enums.OrderEnumStatus.OrderStatus.Delivered)
                    .CountAsync();
            }
            else
            {
                return _context.Orders
                    .Where(o => o.status == Enums.OrderEnumStatus.OrderStatus.Delivered)
                    .CountAsync();
            }
        }

        public async Task<List<ListOrderDto>> ListOrderDto(DateTime? startDate, DateTime? endDate)
        {
            var query = await _context.Orders
                .Include(x => x.User)
                .ToListAsync();
            if (startDate.HasValue)
            {
                query = query.Where(o => o.order_date >= startDate.Value).ToList();
            }
            if (endDate.HasValue)
            {
                query = query.Where(o => o.order_date <= endDate.Value).ToList();
            }
            var listOrderDto = query.Select(o => new ListOrderDto
            {
                order_id = o.order_id,
                customer_name = o.User != null ? o.User.first_name + o.User.last_name : "Unknown",
                customer_email = o.User != null ? o.User.email : "Unknown",
                created_at = o.order_date,
                status = o.status.ToString(),
                total_amount = o.total_amount
            }).ToList();
            return listOrderDto;
        }

        public async Task UpdateAsync(Order entity)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.order_id == entity.order_id);
            if (order != null)
            {
                order.status = entity.status;
               await _context.SaveChangesAsync();
            }
        }
    }
}
