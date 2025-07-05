using book_shop.Data;
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
            return await _context.Payments.ToListAsync();
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

        public Task<Payment> GetPaymentByMethod(int Id)
        {
            var payment = _context.Payments.FirstOrDefaultAsync(p => p.method_id == Id);
            return payment;
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
