using book_shop.Models;

namespace book_shop.Repositories.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<Address> GetAddressByUserId(int id);
    }
}
