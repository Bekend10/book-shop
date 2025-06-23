using book_shop.Data;
using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly ILogger<AddressService> _logger;
        private readonly IAddressRepository _addressRepository;
        public AddressService(ILogger<AddressService> logger, IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
            _logger = logger;
        }
        public async Task<object> DeleteAddress(int id)
        {
            try
            {
                var address = _addressRepository.GetByIdAsync(id);
                if (address == null)
                {
                    _logger.LogInformation("Không tìm thấy địa chỉ id {id}", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy địa chỉ !"
                    };
                }
                await _addressRepository.DeleteAsync(id);
                _logger.LogInformation("Xoá thành công địa chỉ id {id}", id);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Xoá thành công địa chỉ !"
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lỗi " + ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi xảy ra " + ex.Message
                };
            }
        }

        public async Task<object> GetAddressById(int id)
        {
            try
            {
                var address = _addressRepository.GetByIdAsync(id);
                if (address == null)
                {
                    _logger.LogInformation("Không tìm thấy địa chỉ id {id}", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy địa chỉ !"
                    };
                }
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy địa chỉ thành công !",
                    data = address
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lỗi " + ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi xảy ra " + ex.Message
                };
            }
        }

        public async Task<object> GetAllAddresses()
        {
            var addresses = await _addressRepository.GetAllAsync();
            return new
            {
                status = HttpStatusCode.OK,
                msg = "Lấy danh sách địa chỉ thành công !",
                data = addresses
            };
        }

        public async Task<object> UpdateAddress(int id, UpdateAddressDto address)
        {
            try
            {
                var isExistingAddress = await _addressRepository.GetByIdAsync(id);
                if (address == null)
                {
                    _logger.LogInformation("Không tìm thấy địa chỉ id {id}", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy địa chỉ !"
                    };
                }
                if (address.country != null)
                {
                    isExistingAddress.country = address.country;
                }
                if (address.councious != null)
                {
                    isExistingAddress.country = address.councious;
                }
                if (address.district != null)
                {
                    isExistingAddress.country = address.district;
                }
                if (address.commune != null)
                {
                    isExistingAddress.country = address.commune;
                }
                if (address.house_number != null)
                {
                    isExistingAddress.country = address.house_number;
                }

                await _addressRepository.UpdateAsync(isExistingAddress);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Cập nhật địa chỉ thành công !",
                    data = isExistingAddress
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lỗi " + ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi xảy ra " + ex.Message
                };
            }
        }
    }
}
