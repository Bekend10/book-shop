using book_shop.Data;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Address entity)
        {
            await _context.Address.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var address = await _context.Address.FindAsync(id);
            if (address != null)
            {
                _context.Address.Remove(address);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _context.Address.ToListAsync();
        }

        public async Task<Address> GetByIdAsync(int id)
        {
            return await _context.Address.FirstOrDefaultAsync(_ => _.address_id == id);
        }

        public async Task UpdateAsync(Address entity)
        {
            var address = await GetByIdAsync(entity.address_id);
            if (address != null)
            {
                _context.Address.Update(address);
                await _context.SaveChangesAsync();
            }
        }
    }
}
