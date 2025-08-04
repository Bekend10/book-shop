using book_shop.Data;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class BookReviewRepository : IBookReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public BookReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BookReview entity)
        {
            await _context.BookReviews.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _context.BookReviews.FindAsync(id);
            if (review != null)
            {
                _context.BookReviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Review with ID {id} not found.");
            }
        }

        public async Task<IEnumerable<BookReview>> GetAllAsync()
        {
            var list = await _context.BookReviews
                .Include(r => r.book)
                .Include(r => r.user)
                .ToListAsync();
            return list;
        }

        public async Task<double> GetAverageRatingByBookId(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.bookReviews)
                .FirstOrDefaultAsync(b => b.book_id == bookId);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} not found.");
            }
            if (book.bookReviews.Count == 0)
            {
                return 0;
            }
            return book.bookReviews.Average(r => r.rating);
        }

        public async Task<BookReview> GetByIdAsync(int id)
        {
            var review = await _context.BookReviews.FindAsync(id);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {id} not found.");
            }
            return review;
        }

        public async Task<int> GetReviewCountByBookIdAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.bookReviews)
                .FirstOrDefaultAsync(b => b.book_id == bookId);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} not found.");
            }
            return book.bookReviews.Count;
        }

        public async Task<IEnumerable<BookReview>> GetReviewsByBookIdAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.bookReviews)
                .ThenInclude(br => br.user)
                .FirstOrDefaultAsync(b => b.book_id == bookId);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} not found.");
            }
            return book.bookReviews;
        }

        public async Task<IEnumerable<BookReview>> GetReviewsByUserIdAsync(int userId)
        {
            var reviews =  await   _context.BookReviews
                .Where(r => r.user_id == userId)
                .Include(r => r.book)
                .ToListAsync();
            return reviews;
        }

        public async Task UpdateAsync(BookReview entity)
        {
            var existingReview = await _context.BookReviews.FindAsync(entity.book_review_id);
            if (existingReview != null)
            {
                existingReview.book_id = entity.book_id;
                existingReview.user_id = entity.user_id;
                existingReview.image = entity.image;
                existingReview.rating = entity.rating;
                existingReview.content = entity.content;
                existingReview.created_at = entity.created_at;

                _context.BookReviews.Update(existingReview);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Review with ID {entity.book_review_id} not found.");
            }
        }
    }
}
