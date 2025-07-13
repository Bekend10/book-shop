using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Implementations;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;

        public UserService(IUserRepository userRepository, IAddressRepository addressRepository)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await _userRepository.GetAllAsync();

        public async Task<object> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new
                {
                    status = HttpStatusCode.NotFound,
                    msg = "Không tìm thấy người dùng!"
                };
            }
            var address = await _addressRepository.GetByIdAsync(user.address_id);
            if (address != null)
            {
                user.Address = address;
            }
            return new
            {
                status = HttpStatusCode.OK,
                msg = "Lấy thông tin người dùng thành công !",
                data = new UserRespone
                {
                    user_id = user.user_id,
                    email = user.email,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    phone_number = user.phone_number,
                    profile_image = user.profile_image,
                    full_name = $"{user.first_name} {user.last_name}",
                    address = user.Address,
                }
            };
        }

        public async Task<object> CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                first_name = dto.first_name,
                last_name = dto.last_name,
                email = dto.email,
                dob = dto.dob,
                gender = dto.gender,
                phone_number = dto.phone_number,
                profile_image = dto.profile_image,
                created_at = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            return new { status = HttpStatusCode.Created, msg = "Thêm người dùng thành công!" };
        }

        public async Task<object> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy người dùng!" };

            bool isUpdated = false;

            if (!string.IsNullOrWhiteSpace(dto.first_name) && dto.first_name != user.first_name)
            {
                user.first_name = dto.first_name;
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.last_name) && dto.last_name != user.last_name)
            {
                user.last_name = dto.last_name;
                isUpdated = true;
            }
            if (!string.IsNullOrWhiteSpace(dto.country) && dto.country != user.Address.country)
            {
                user.Address.country = dto.country;
                isUpdated = true;
            }
            if (!string.IsNullOrWhiteSpace(dto.councious) && dto.councious != user.Address.councious)
            {
                user.Address.councious = dto.councious;
                isUpdated = true;
            }
            if (!string.IsNullOrWhiteSpace(dto.commune) && dto.commune != user.Address.commune)
            {
                user.Address.commune = dto.commune;
                isUpdated = true;
            }
            if (!string.IsNullOrWhiteSpace(dto.district) && dto.district != user.Address.district)
            {
                user.Address.district = dto.district;
                isUpdated = true;
            }
            if (!string.IsNullOrWhiteSpace(dto.house_number) && dto.house_number != user.Address.house_number)
            {
                user.Address.house_number = dto.house_number;
                isUpdated = true;
            }

            if (dto.dob.HasValue && dto.dob.Value.Date != user.dob.Date)
            {
                user.dob = (DateTime)dto.dob;
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.phone_number) && dto.phone_number != user.phone_number)
            {
                user.phone_number = dto.phone_number;
                isUpdated = true;
            }

            if (dto.gender.HasValue && dto.gender != user.gender)
            {
                user.gender = (bool)dto.gender;
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.profile_image) && dto.profile_image != user.profile_image)
            {
                user.profile_image = dto.profile_image;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return new { status = HttpStatusCode.NotModified, msg = "Không có thay đổi nào được thực hiện." };
            }

            await _userRepository.UpdateAsync(user);
            return new { status = HttpStatusCode.OK, msg = "Cập nhật thành công!" };
        }


        public async Task<object> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy người dùng!" };

            await _userRepository.DeleteAsync(id);
            return new { status = HttpStatusCode.OK, msg = "Xoá người dùng thành công!" };
        }

        public async Task<object> GetMyInformation()
        {
            var userId = await _userRepository.GetCurrentUserIdAsync();
            if (userId < 0)
            {
                return new
                {
                    status = HttpStatusCode.NotFound,
                    msg = "Không tìm thấy người dùng"
                };
            }

            var infor = await _userRepository.GetByIdAsync(userId);
            return new
            {
                status = HttpStatusCode.OK,
                msg = "Lấy thông tin người dùng thành công !",
                infor
            };
        }
    }

}
