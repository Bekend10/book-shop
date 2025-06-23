using book_shop.Data;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class CategoryReponsitory : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryReponsitory(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Category entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cate = await _context.Categories.FindAsync(id);
            if (cate != null)
            {
                _context.Categories.Remove(cate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Select(_ => new Category
                {
                    category_id = _.category_id,
                    name = _.name,
                    description = _.description,
                    created_at = _.created_at,
                    created_by = _.created_by
                })
                .ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Where(x => x.category_id == id)
                .Select(_ => new Category
                {
                    category_id = _.category_id,
                    name = _.name,
                    description = _.description,
                    created_at = _.created_at,
                    created_by = _.created_by
                }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesByBookIdAndCategoryIdAsync(int bookId, int categoryId)
        {
            return await _context.Categories
                .Where(c => c.category_id == categoryId && c.book.Any(b => b.book_id == bookId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesByBookIdAndCategoryIdsAsync(int bookId, IEnumerable<int> categoryIds)
        {
            return await _context.Categories
                .Where(c => categoryIds.Contains(c.category_id) && c.book.Any(b => b.book_id == bookId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesByBookIdAsync(int bookId)
        {
            return await _context.Categories
                .Where(c => c.book.Any(b => b.book_id == bookId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesByBookIdsAsync(IEnumerable<int> bookIds)
        {
            return await _context.Categories
                .Where(c => c.book.Any(b => bookIds.Contains(b.book_id)))
                .ToListAsync();
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _context.Categories
                .Where(c => c.name == name)
                .Select(_ => new Category
                {
                    category_id = _.category_id,
                    name = _.name,
                    description = _.description,
                    created_at = _.created_at,
                    created_by = _.created_by
                }).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Category entity)
        {
            _context.Categories.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
