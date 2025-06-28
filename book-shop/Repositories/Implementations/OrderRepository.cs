using book_shop.Data;
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

        public async Task UpdateAsync(Order entity)
        {
            var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.order_id == entity.order_id);
            if (order != null)
            {
                order.status = entity.status;
               await _context.SaveChangesAsync();
            }
        }
    }
}
