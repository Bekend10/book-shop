using book_shop.Dto;
using book_shop.Models;
using System;

namespace book_shop.Repositories.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId);
        Task<IEnumerable<Book>> GetBooksByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Book>> GetBooksByPublisherAsync(string publisher);
        Task<IEnumerable<Book>> GetBooksByTitleAsync(string title);
        Task<List<TopProductDto>> GetTopProducts(DateTime? startDate, DateTime? endDate);
        Task<List<Book>> GetByIdsAsync(IEnumerable<int> bookIds);
        Task<bool> ImportBookByExcel(List<AddBookByExcelModel> books);
    }
}
