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

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await _userRepository.GetAllAsync();

        public async Task<User?> GetByIdAsync(int id) => await _userRepository.GetByIdAsync(id);

        public async Task<object> CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                first_name = dto.first_name,
                last_name = dto.last_name,
                email = dto.email,
                dob = dto.dob,
                gender = dto.gender,
                profile_image = dto.profile_image,
                address_id = dto.address_id,
                created_at = DateTime.UtcNow
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

            if (dto.dob.HasValue && dto.dob.Value.Date != user.dob.Date)
            {
                user.dob = (DateTime)dto.dob;
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

            if (dto.address_id.HasValue && dto.address_id != user.address_id)
            {
                user.address_id = (int)dto.address_id;
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
    }

}
