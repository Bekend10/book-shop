using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<object> CreateAsync(CreateRoleDto dto);
        Task<object> UpdateAsync(int id, UpdateRoleDto dto);
        Task<object> DeleteAsync(int id);
    }
}
