using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class MethodService : IMethodService
    {
        private readonly IMethodRepository _methodRepository;
        private readonly ILogger<MethodService> _logger;

        public MethodService(IMethodRepository methodRepository, ILogger<MethodService> logger)
        {
            _methodRepository = methodRepository;
            _logger = logger;
        }

        public async Task<object> AddMethod(MethodDto method)
        {
            try
            {
                var newMethod = new Method
                {
                    method_id = method.method_id,
                    method_name = method.method_name,
                    description = method.description
                };
                await _methodRepository.AddAsync(newMethod);
                _logger.LogInformation("Thêm phương thức thanh toán thành công với ID: {MethodId}", method.method_id);
                return new
                {
                    status = HttpStatusCode.Created,
                    message = "Thêm phương thức thanh toán thành công",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm phương thức thanh toán với ID: {MethodId}", method.method_id);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi thêm phương thức thanh toán: " + ex.Message
                };
            }
        }

        public async Task<object> DeleteMethod(int id)
        {
            try
            {
                var method = await _methodRepository.GetByIdAsync(id);
                if (method == null)
                {
                    _logger.LogWarning("Phương thức thanh toán không tồn tại với ID: {MethodId}", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Phương thức thanh toán không tồn tại"
                    };
                }
                await _methodRepository.DeleteAsync(id);
                _logger.LogInformation("Xoá phương thức thanh toán thành công với ID: {MethodId}", id);
                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Xoá phương thức thanh toán thành công"
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xoá phương thức thanh toán với ID: {MethodId}", id);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi xoá phương thức thanh toán: " + ex.Message
                };
            }
        }

        public async Task<object> GetAllMethods()
        {
            var methods = await _methodRepository.GetAllAsync();
            if (methods == null || !methods.Any())
            {
                _logger.LogWarning("Không tìm thấy phương thức thanh toán nào");
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Không tìm thấy phương thức thanh toán nào"
                };
            }
            _logger.LogInformation("Lấy danh sách phương thức thanh toán thành công");
            return new
            {
                status = HttpStatusCode.OK,
                message = "Lấy danh sách phương thức thanh toán thành công",
                data = methods
            };
        }

        public async Task<object> GetMethodById(int id)
        {
            var method = await _methodRepository.GetByIdAsync(id);
            if (method == null)
            {
                _logger.LogWarning("Không tìm thấy phương thức thanh toán với ID: {MethodId}", id);
                return new
                {
                    status = HttpStatusCode.NotFound,
                    message = "Không tìm thấy phương thức thanh toán"
                };
            }
            _logger.LogInformation("Lấy phương thức thanh toán thành công với ID: {MethodId}", id);
            return new
            {
                status = HttpStatusCode.OK,
                message = "Lấy phương thức thanh toán thành công",
                data = method
            };

        }

        public async Task<object> UpdateMethod(int id, UpdateMethodDto method)
        {
            try
            {
                var existingMethod = await _methodRepository.GetByIdAsync(id);
                if (existingMethod == null)
                {
                    _logger.LogWarning("Phương thức thanh toán không tồn tại với ID: {MethodId}", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Phương thức thanh toán không tồn tại"
                    };
                }
                if (method.method_name != null)
                    existingMethod.method_name = method.method_name;

                if (method.description != null)
                    existingMethod.description = method.description;

                await _methodRepository.UpdateAsync(existingMethod);
                _logger.LogInformation("Sửa phương thức thanh toán thành công với ID: {MethodId}", id);
                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Sửa phương thức thanh toán thành công",
                    data = existingMethod
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi sửa phương thức thanh toán với ID: {MethodId}", id);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi khi sửa phương thức thanh toán: " + ex.Message
                };
            }
        }
    }
}
