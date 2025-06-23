using book_shop.Data;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class MethodRepository : IMethodRepository
    {
        private readonly ApplicationDbContext _context;

        public MethodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Method entity)
        {
            await _context.Methods.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var method = _context.Methods.Find(id);
            if (method != null)
            {
                _context.Methods.Remove(method);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Method>> GetAllAsync()
        {
            return await _context.Methods.Select(_ => new Method
            {
                method_id = _.method_id,
                method_name = _.method_name,
                description = _.description,
            }).ToListAsync();
        }

        public async Task<Method> GetByIdAsync(int id)
        {
            var method = await _context.Methods
                .Select(_ => new Method
                {
                    method_id = _.method_id,
                    method_name = _.method_name,
                    description = _.description,
                })
                .FirstOrDefaultAsync(m => m.method_id == id);
            return method;
        }

        public async Task UpdateAsync(Method entity)
        {
            var method = await _context.Methods.FirstOrDefaultAsync(m => m.method_id == entity.method_id);
            if (method != null)
            {
                if (entity.method_name != null)
                    method.method_name = entity.method_name;

                if (entity.description != null)
                    method.description = entity.description;

                await _context.SaveChangesAsync();
            }
        }
    }
}
