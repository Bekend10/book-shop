using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;

namespace book_shop.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<object> GetAuthorByNationally(string nationally);
        Task<object> GetAuthorById(int id);
        Task<object> GetAuthors();
        Task<object> CreateAuthor(AuthorDto author);
        Task<object> UpdateAuthor(int id, UpdateAuthorDto author);
        Task<object> DeleteAuthor(int id);
        Task<object> GetAuthorByBook(int bookId);
    }
}
