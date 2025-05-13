using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> GetCategoryByNameAsync(string name);
        Task<IEnumerable<Category>> GetCategoriesByBookIdAsync(int bookId);
        Task<IEnumerable<Category>> GetCategoriesByBookIdsAsync(IEnumerable<int> bookIds);
        Task<IEnumerable<Category>> GetCategoriesByBookIdAndCategoryIdAsync(int bookId, int categoryId);
        Task<IEnumerable<Category>> GetCategoriesByBookIdAndCategoryIdsAsync(int bookId, IEnumerable<int> categoryIds);
    }
}
