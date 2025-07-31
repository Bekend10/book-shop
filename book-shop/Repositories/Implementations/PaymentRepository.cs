using book_shop.Data;
using book_shop.Dto;
using book_shop.Enums;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Payment entity)
        {
            await _context.Payments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(x => x.order)
                .ThenInclude(x => x.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync()
        {
            var payments = await _context.Payments
                .Include(p => p.order)
                .ThenInclude(o => o.User)
                .Select(p => new PaymentDto
                {
                    PaymentId = p.payment_id,
                    OrderId = p.order_id,
                    MethodId = p.method_id,
                    Amount = p.amount,
                    PaymentStatus = p.payment_status,
                    PaymentDate = p.order.order_date,
                    User = new UserDto
                    {
                        user_id = p.order.User.user_id,
                        first_name = p.order.User.first_name,
                        email = p.order.User.email,
                        last_name = p.order.User.last_name
                    }
                })
                .ToListAsync();
            return payments;
        }

        public async Task<Payment> GetByIdAsync(int id)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(_ => _.payment_id == id);
            return payment;
        }

        public Task<Payment> GetByOrderId(int id)
        {
            var payment = _context.Payments.FirstOrDefaultAsync(p => p.order_id == id);
            return payment;
        }

        public Task<List<MonthlyRevenueDto>> GetMonthlyRevenue(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Payments
                .Where(p => p.payment_status == Enums.PaymentEnumStatus.Completed);

            if (startDate.HasValue)
            {
                query = query.Where(p => p.payment_date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.payment_date <= endDate.Value);
            }

            return query
                .GroupBy(p => new { p.payment_date.Year, p.payment_date.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    month = g.Key.Month,
                    revenue = g.Sum(p => p.amount)
                })
                .ToListAsync();
        }


        public Task<Payment> GetPaymentByMethod(int Id)
        {
            var payment = _context.Payments.FirstOrDefaultAsync(p => p.method_id == Id);
            return payment;
        }

        public Task<PaymentStatus> GetPaymentStatus(int id)
        {
            return _context.Payments
                .Where(p => p.order_id == id)
                .Select(p => new PaymentStatus
                {
                    PaymentId = p.payment_id,
                    Status = p.payment_status
                })
                .FirstOrDefaultAsync();
        }

        public Task<int> GetPaymentTotalByStatus()
        {
            return _context.Payments
                .Where(p => p.payment_status == PaymentEnumStatus.Completed)
                .SumAsync(p => p.amount);
        }

        public async Task UpdateAsync(Payment entity)
        {
            var payment = await _context.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.payment_id == entity.payment_id);
            if (payment != null)
            {
                _context.Entry(payment).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
