using book_shop.Dto;
using book_shop.Models;

namespace book_shop.Services.Interfaces
{
    public interface IAddressService
    {
        Task<object> GetAllAddresses();
        Task<object> GetAddressById(int id);
        Task<object> UpdateAddress(int id, UpdateAddressDto address);
        Task<object> DeleteAddress(int id);
    }
}
