using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;
        public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger)
        {
            _logger = logger;
            _roleRepository = roleRepository;
        }
        public async Task<object> CreateAsync(CreateRoleDto dto)
        {
            try
            {
                var existingRole = await _roleRepository.GetRoleByName(dto.role_name);
                if (existingRole != null)
                {
                    _logger.LogInformation("Thêm quyền {role} thất bại vì đã tồn tại !", dto.role_name);
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        msg = "Quyền đã tồn tại !"
                    };
                }
                var role = new Role
                {
                    role_name = dto.role_name,
                };
                await _roleRepository.AddAsync(role);
                _logger.LogInformation("Thêm quyền {role} thành công !", dto.role_name);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Thêm quyền thành công !"
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Thêm quyền {role} thất bại vì {ex}", dto.role_name, ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message
                };
            }
        }

        public async Task<object> DeleteAsync(int id)
        {
            try
            {
                var existingRole = _roleRepository.GetByIdAsync(id);
                if (existingRole == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy quyền !"
                    };
                }
                await _roleRepository.DeleteAsync(id);
                _logger.LogInformation("Xoá quyền thành công !");
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Xoá quyền thành công !"
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Xoá quyền thất bại vì {ex}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message
                };
            }
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<object> UpdateAsync(int id, UpdateRoleDto dto)
        {
            var existingRole = await _roleRepository.GetByIdAsync(id);
            if (existingRole == null)
            {
                return new
                {
                    status = HttpStatusCode.NotFound,
                    msg = "Không tìm thấy quyền !"
                };
            }

            if (dto == null)
            {
                return new
                {
                    status = HttpStatusCode.BadRequest,
                    msg = "Chưa điền thông tin !"
                };
            }
            else
            {
                if (existingRole.role_name == dto.role_name)
                {
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        msg = "Tên quyền đã tồn tại !"
                    };
                }
                else
                {
                    existingRole.role_name = dto.role_name;
                    await _roleRepository.UpdateAsync(existingRole);
                    return new
                    {
                        status = HttpStatusCode.OK,
                        msg = "Sửa quyền thành công !"
                    };
                }
            }
        }
    }
}
