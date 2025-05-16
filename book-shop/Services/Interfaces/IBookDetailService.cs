using book_shop.Dto;

namespace book_shop.Services.Interfaces
{
    public interface IBookDetailService
    {
        Task<object> GetBookDetailById(int id);
        Task<object> AddBookDetail(BookDetailDto bookDetailDto);
        Task<object> UpdateBookDetail(int id, BookDetailDto bookDetailDto , IFormFile image);
        Task<object> DeleteBookDetail(int id);
        Task<object> GetBookDetailByBookId(int bookId);
    }
}
