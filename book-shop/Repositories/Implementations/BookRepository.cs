using book_shop.Data;
using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        public BookRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task AddAsync(Book entity)
        {
            await _context.Books.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.Include(b => b.authors)
                .Include(b => b.category)
                .Include(b => b.bookReviews)
                .Include(b => b.bookDetail)
                .Include(b => b.orderDetail)
                .Include(b => b.authors)
                .Include(b => b.cartDetails)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            var books = await _context.Books
                .Include(b => b.category)
                .Include(b => b.bookReviews)
                .Include(b => b.bookDetail)
                .Include(b => b.orderDetail)
                .Include(b => b.authors)
                .Include(b => b.cartDetails)
                .Where(c => c.author_id == authorId)
                .ToListAsync();
            return books;
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryIdAsync(int categoryId)
        {
            var books = await _context.Books
                .Include(b => b.category)
                .Include(b => b.bookReviews)
                .Include(b => b.bookDetail)
                .Include(b => b.orderDetail)
                .Include(b => b.authors)
                .Include(b => b.cartDetails)
                .Where(c => c.category_id == categoryId)
                .ToListAsync();
            return books;
        }

        public async Task<IEnumerable<Book>> GetBooksByPublisherAsync(string publisher)
        {
            var books = await _context.Books
                .Include(b => b.category)
                .Include(b => b.bookReviews)
                .Include(b => b.bookDetail)
                .Include(b => b.orderDetail)
                .Include(b => b.authors)
                .Include(b => b.cartDetails)
                .Where(c => c.publisher == publisher)
                .ToListAsync();
            return books;
        }
       
        public async Task<IEnumerable<Book>> GetBooksByTitleAsync(string title)
        {
            var books = await _context.Books
                .Include(b => b.category)
                .Include(b => b.bookReviews)
                .Include(b => b.bookDetail)
                .Include(b => b.orderDetail)
                .Include(b => b.authors)
                .Include(b => b.cartDetails)
                .Where(c => c.title.Contains(title))
                .ToListAsync();
            return books;
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.book_id == id);
            return book;
        }

        public async Task<List<Book>> GetByIdsAsync(IEnumerable<int> bookIds)
        {
            return await _context.Books.Where(b => bookIds.Contains(b.book_id)).ToListAsync();
        }

        public async Task<List<TopProductDto>> GetTopProducts(DateTime? startDate, DateTime? endDate)
        {
            var orderDetails = _context.OrderDetails
                .Include(od => od.book)
                .ThenInclude(b => b.category)
                .Include(od => od.order)
                .Where(od => od.order.status == Enums.OrderEnumStatus.OrderStatus.Delivered || od.order.status == Enums.OrderEnumStatus.OrderStatus.Processing);

            if (startDate.HasValue)
            {
                orderDetails = orderDetails.Where(od => od.order.order_date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                orderDetails = orderDetails.Where(od => od.order.order_date <= endDate.Value);
            }

            var result = await orderDetails
                .GroupBy(od => od.book_id)
                .Select(g => new TopProductDto
                {
                    product_id = g.Key,
                    name = g.Select(x => x.book.title).FirstOrDefault(),
                    image = g.Select(x => x.book.image_url).FirstOrDefault(),
                    category_name = g.Select(x => x.book.category.name).FirstOrDefault(),
                    quantity_sold = g.Sum(x => x.quantity),
                    revenue = g.Sum(x => x.quantity * x.book.price),
                    total_quantity = g.Select(x => x.book.quantity).FirstOrDefault(),
                    author = g.Select(x => x.book.authors.FirstOrDefault()).FirstOrDefault(),
                })
                .OrderByDescending(tp => tp.quantity_sold)
                .ToListAsync();

            return result;
        }


        public async Task UpdateAsync(Book entity)
        {
            var book = await _context.Books.FindAsync(entity.book_id);
            if (book != null)
            {
                book.title = entity.title;
                book.author_id = entity.author_id;
                book.quantity = entity.quantity;
                book.publisher = entity.publisher;
                book.price = entity.price;
                book.price_origin = entity.price_origin;
                book.category_id = entity.category_id;
                book.image_url = entity.image_url;
                _context.Books.Update(book);
                _context.SaveChanges();
            }
            await _context.SaveChangesAsync();
        }
    }
}
