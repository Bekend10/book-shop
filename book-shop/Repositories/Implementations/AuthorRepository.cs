using book_shop.Data;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Author entity)
        {
            _context.Authors.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _context.Authors
                .Include(b => b.books)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Author> GetAuthorByNationally(string nationally)
        {
            return await _context.Authors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.nationally == nationally);
        }

        public async Task<Author> GetAuthorsByBookId(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            var author = await _context.Authors
                .FirstOrDefaultAsync(_ => _.author_id == book.author_id);
            return author;
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            return await _context.Authors
                .Include(b => b.books)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.author_id == id);
        }

        public async Task UpdateAsync(Author entity)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.author_id == entity.author_id);
            if (author != null)
            {
                _context.Entry(author).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
