using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IBookReviewRepository : IRepository<BookReview>
    {
        Task<int> GetReviewCountByBookIdAsync(int bookId);
        Task<double> GetAverageRatingByBookId(int bookId);
        Task<IEnumerable<BookReview>> GetReviewsByBookIdAsync(int bookId);
        Task<IEnumerable<BookReview>> GetReviewsByUserIdAsync(int userId);
    }
}
