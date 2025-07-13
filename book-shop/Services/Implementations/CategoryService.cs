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
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<CategoryService> _logger;
        private readonly UserHelper _userHelper;
        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger, UserHelper userHelper, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _userHelper = userHelper;
            _bookRepository = bookRepository;
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
            var categoryRespone = result.Select(c => new CategoryDto
            {
                category_id = c.category_id,
                name = c.name,
                description = c.description,
                created_at = c.created_at,
                created_by = c.created_by
            }).ToList();
            foreach (var item in categoryRespone)
            {
                var bookList = await _bookRepository.GetBooksByCategoryIdAsync(item.category_id);
                item.book_count = bookList.Count();
            }

            _logger.LogInformation("Lấy danh sách danh mục thành công !");
            return new {status = HttpStatusCode.OK, msg = "Lấy danh sách danh mục thành công !" ,data = categoryRespone };
        }


        public async Task<object> GetCategoryByIdAsync(int id)
        {
            var cate = await _categoryRepository.GetByIdAsync(id);
            if (cate == null)
            {
                _logger.LogInformation("Không tìm thấy danh mục với id {id}", id);
                return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy danh mục !" };
            }
            var bookList = await _bookRepository.GetBooksByCategoryIdAsync(id);
            var bookCount = bookList.Count();
            _logger.LogInformation("Lấy danh mục {category} thành công !", cate.name);
            return new { status = HttpStatusCode.OK, msg = "Lấy danh mục thành công !", data = cate , book_count = bookCount};
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
