using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IBookDetailRepository : IRepository<BookDetail>
    {
        Task<BookDetail> GetBookDetailsByBookIdAsync(int bookId);
    }
}
