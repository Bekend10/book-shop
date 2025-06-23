using book_shop.Dto;
using book_shop.Helpers.UserHelper;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;
        private readonly UserHelper _userHelper;
        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger , UserHelper userHelper)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _userHelper = userHelper;
        }
        public async Task<object> AddCategoryAsync(CategoryDto category)
        {
            try
            {
                var userID = _userHelper.GetCurrentUserId();
                if(userID == -1)
                {
                    _logger.LogInformation("Thêm danh mục {category} thất bại vì không tìm thấy người dùng !", category.name);
                    return new { status = HttpStatusCode.BadRequest, message = "Không tìm thấy người dùng !" };
                }

                var isExist = await _categoryRepository.GetCategoryByNameAsync(category.name);
                if(isExist != null)
                {
                    _logger.LogInformation("Thêm danh mục {category} thất bại vì đã tồn tại !", category.name);
                    return new { status = HttpStatusCode.BadRequest, message = "Danh mục đã tồn tại !" };
                }

                var newCategory = new Category
                {
                    name = category.name,
                    description = category.description,
                    created_at = DateTime.Now,
                    created_by = userID,
                };
                await _categoryRepository.AddAsync(newCategory);
                _logger.LogInformation("Thêm danh mục {category} thành công !", category.name);
                return new { status = HttpStatusCode.Created, message = "Thêm danh mục thành công !" };
            }
            catch (Exception ex)
            {
                return new { status = HttpStatusCode.InternalServerError, message = ex.Message };
            }
        }

        public async Task<object> DeleteCategoryAsync(int id)
        {
            var cate = await _categoryRepository.GetByIdAsync(id);
            if (cate == null)
            {
                return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy danh mục !" };
            }
            await _categoryRepository.DeleteAsync(id);
            return new { status = HttpStatusCode.OK, msg = "Xóa danh mục thành công !" };
        }

        public async Task<object> GetAllCategoriesAsync()
        {
            var result = await _categoryRepository.GetAllAsync();
            if (result == null)
            {
                _logger.LogInformation("Không tìm thấy danh mục nào !");
                return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy danh mục nào !" };
            }
            _logger.LogInformation("Lấy danh sách danh mục thành công !");
            return new {status = HttpStatusCode.OK, msg = "Lấy danh sách danh mục thành công !" ,data = result };
        }


        public async Task<object> GetCategoryByIdAsync(int id)
        {
            var cate = await _categoryRepository.GetByIdAsync(id);
            if (cate == null)
            {
                _logger.LogInformation("Không tìm thấy danh mục với id {id}", id);
                return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy danh mục !" };
            }
            _logger.LogInformation("Lấy danh mục {category} thành công !", cate.name);
            return new { status = HttpStatusCode.OK, msg = "Lấy danh mục thành công !", data = cate };
        }

        public async Task<object> UpdateCategoryAsync(int id, CategoryDto category)
        {
            var cate = await _categoryRepository.GetByIdAsync(id);
            if (cate == null)
            {
                return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy danh mục !" };
            };
            cate.name = category.name;
            cate.description = category.description;
            await _categoryRepository.UpdateAsync(cate);
            _logger.LogInformation("Cập nhật danh mục {category} thành công !", category.name);
            return new { status = HttpStatusCode.OK, msg = "Cập nhật danh mục thành công !" };
        }
    }
}
