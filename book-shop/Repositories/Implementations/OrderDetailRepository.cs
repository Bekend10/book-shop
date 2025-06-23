using book_shop.Data;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(OrderDetail entity)
        {
            await _context.OrderDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            var orderDetails = await _context.OrderDetails
                .Select(od => new OrderDetail
                {
                    order_detail_id = od.order_detail_id,
                    order_id = od.order_id,
                    book_id = od.book_id,
                    quantity = od.quantity,
                    unit_price = od.unit_price
                })
                .ToListAsync();
            return orderDetails;
        }

        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            var orderDetail = await _context.OrderDetails
                .Select(od => new OrderDetail
                {
                    order_detail_id = od.order_detail_id,
                    order_id = od.order_id,
                    book_id = od.book_id,
                    quantity = od.quantity,
                    unit_price = od.unit_price
                })
                .FirstOrDefaultAsync(od => od.order_detail_id == id);
            return orderDetail;
        }

        public async Task<object> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.order_id == orderId)
                .Select(od => new OrderDetail
                {
                    order_detail_id = od.order_detail_id,
                    order_id = od.order_id,
                    book_id = od.book_id,
                    quantity = od.quantity,
                    unit_price = od.unit_price
                })
                .FirstOrDefaultAsync();
            return orderDetails;
        }

        public async Task UpdateAsync(OrderDetail entity)
        {
            var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(od => od.order_detail_id == entity.order_detail_id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
