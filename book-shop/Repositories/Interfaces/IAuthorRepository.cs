using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<Author> GetAuthorByNationally(string nationally);
        Task<Author> GetAuthorsByBookId(int bookId);
    }
}
