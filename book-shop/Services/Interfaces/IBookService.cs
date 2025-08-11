using book_shop.Dto;
using book_shop.Models;
using Microsoft.AspNetCore.Mvc;

namespace book_shop.Services.Interfaces
{
    public interface IBookService
    {
        Task<object> GetAllBooksAsync();
        Task<object> GetBookByIdAsync(int id);
        Task<object> AddBook([FromForm] AddBookDto request);
        Task<object> UpdateBookAsync(int id , [FromForm] UpdateBookDto request);
        Task<object> DeleteBookAsync(int id);
        Task<object> GetBooksByAuthorIdAsync(int authorId);
        Task<object> GetBooksByCategoryIdAsync(int categoryId);
        Task<object> GetBooksByPublisherAsync(string publisher);
        Task<object> GetBooksByTitleAsync(string title);
        Task<object> GetTopProductsAsync(DateTime? startDate, DateTime? endDate);
        Task<ImportBooksResult> ImportBooksFromExcelAsync(IFormFile file);
        Task<byte[]> GetBooksImportTemplateAsync();
    }
}
