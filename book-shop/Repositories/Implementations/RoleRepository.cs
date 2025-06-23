using book_shop.Data;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Role entity)
        {
            await _context.Roles.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if(role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Role> GetRoleByName(string role_name)
        {
            return await _context.Roles.FirstOrDefaultAsync(_ => _.role_name == role_name);
        }

        public async Task UpdateAsync(Role entity)
        {
            _context.Roles.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
