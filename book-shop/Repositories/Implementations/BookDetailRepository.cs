using book_shop.Data;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class BookDetailRepository : IBookDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public BookDetailRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task AddAsync(BookDetail entity)
        {
            await _context.BookDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public  async Task DeleteAsync(int id)
        {
            var bookDetail = await _context.BookDetails.FindAsync(id);
            if (bookDetail != null)
            {
                _context.BookDetails.Remove(bookDetail);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<BookDetail>> GetAllAsync()
        {
            var bookDetails = await _context.BookDetails
                .Include(b => b.book)
                .ToListAsync();
            return bookDetails;
        }

        public async Task<BookDetail> GetBookDetailsByBookIdAsync(int bookId)
        {
            return await _context.BookDetails
                                 .Include(b => b.book) 
                                 .FirstOrDefaultAsync(b => b.book_id == bookId);
        }



        public Task<BookDetail> GetByIdAsync(int id)
        {
            var bookDetail = _context.BookDetails
                .Include(b => b.book)
                .FirstOrDefaultAsync(b => b.detail_id == id);
            return bookDetail;
        }

        public async Task UpdateAsync(BookDetail entity)
        {
            var bookDetail = await _context.BookDetails.FindAsync(entity.detail_id);
            if (bookDetail != null)
            {
                bookDetail.book_id = entity.book_id;
                bookDetail.description = entity.description;
                bookDetail.language = entity.language;
                bookDetail.number_of_page = entity.number_of_page;
                bookDetail.price = entity.price;
                bookDetail.file_demo_url = entity.file_demo_url;
                await  _context.SaveChangesAsync();
            }
        }
    }
}
