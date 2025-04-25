using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> GetRoleByName(string role_name);
    }
}
