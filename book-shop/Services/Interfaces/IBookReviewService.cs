using book_shop.Models;
using book_shop.Dto;

namespace book_shop.Services.Interfaces
{
    public interface IBookReviewService
    {
        Task<object> GetReview(int id);
        Task<object> GetAllReviews();
        Task<object> AddReview(BookReviewCreateDto bookReview);
        Task<object> UpdateReview(BookReviewUpdateDto bookReview);
        Task<object> DeleteReview(int id);
        Task<object> GetReviewsByBookId(int bookId);
        Task<object> GetReviewsByUserId(int userId);
        Task<object> GetAverageRatingByBookId(int bookId);
        Task<object> GetReviewCountByBookId(int bookId);
    }
}
