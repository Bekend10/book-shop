using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Services.Interfaces
{
    public interface ICategoryService 
    {
        Task<object> GetAllCategoriesAsync();
        Task<object> GetCategoryByIdAsync(int id);
        Task<object> AddCategoryAsync(CategoryDto category);
        Task<object> UpdateCategoryAsync(int id , CategoryDto category);
        Task<object> DeleteCategoryAsync(int id);
    }
}
