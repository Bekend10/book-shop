using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Implementations;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class BookDetailService : IBookDetailService
    {
        private readonly IBookDetailRepository _bookDetailRepository;
        private readonly ILogger<BookDetailService> _logger;
        private readonly ICloudService _cloudService;

        public BookDetailService(BookDetailRepository bookDetailRepository, ILogger<BookDetailService> logger , ICloudService cloudService)
        {
            _bookDetailRepository = bookDetailRepository;
            _logger = logger;
            _cloudService = cloudService;
        }

        public async Task<object> AddBookDetail(BookDetailDto bookDetailDto)
        {
            try
            {
                var newBookDetail = new Models.BookDetail
                {
                    book_id = bookDetailDto.book_id,
                    description = bookDetailDto.description,
                    language = bookDetailDto.language,
                    number_of_page = bookDetailDto.number_of_page,
                    price = bookDetailDto.price,
                    file_demo_url = bookDetailDto.file_demo_url,
                    create_at = DateTime.Now,
                };
                await _bookDetailRepository.AddAsync(newBookDetail);
                _logger.LogInformation("Thêm chi tiết sách {book_id} thành công !", bookDetailDto.book_id);
                return new { status = HttpStatusCode.Created, msg = "Thêm chi tiết sách thành công !" };
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi thêm chi tiết sách : {message}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Xảy ra lỗi : " + ex.Message
                };
            }

        }
        public async Task<object> DeleteBookDetail(int id)
        {
            var bookDetail = await _bookDetailRepository.GetByIdAsync(id);
            if(bookDetail == null)
            {
                return new { status = HttpStatusCode.NotFound, message = "Không tìm thấy chi tiết sách" };
            }
            await _bookDetailRepository.DeleteAsync(id);
            _logger.LogInformation("Xóa chi tiết sách {id} thành công !", id);
            return new { status = HttpStatusCode.OK, message = "Xóa chi tiết sách thành công !" };
        }

        public async Task<object> GetBookDetailByBookId(int bookId)
        {
            var bookDetail = await _bookDetailRepository.GetBookDetailsByBookIdAsync(bookId);
            if (bookDetail == null)
            {
                return Task.FromResult(new { status = HttpStatusCode.NotFound, message = "Không tìm thấy chi tiết sách" });
            }
            var bookDetailDto = new BookDetailDto
            {
                book_id = bookDetail.book_id,
                description = bookDetail.description,
                language = bookDetail.language,
                number_of_page = bookDetail.number_of_page,
                image_url = bookDetail.image_url,
                price = bookDetail.price,
                file_demo_url = bookDetail.file_demo_url,
                create_at = bookDetail.create_at,
            };
            _logger.LogInformation("Lấy chi tiết sách {book_id} thành công !", bookId);
            return new { status = HttpStatusCode.OK, message = "Lấy chi tiết sách thành công !", data = bookDetailDto };
        }

        public async Task<object> GetBookDetailById(int id)
        {
            var bookDetail = await _bookDetailRepository.GetByIdAsync(id);
            if (bookDetail == null)
            {
                return new { status = HttpStatusCode.NotFound, message = "Không tìm thấy chi tiết sách" };
            }
            var bookDetailDto = new BookDetailDto
            {
                book_id = bookDetail.book_id,
                description = bookDetail.description,
                language = bookDetail.language,
                is_bn = bookDetail.is_bn,
                quantity = bookDetail.quantity,
                number_of_page = bookDetail.number_of_page,
                image_url = bookDetail.image_url,
                price = bookDetail.price,
                file_demo_url = bookDetail.file_demo_url,
                create_at = bookDetail.create_at,
            };
            _logger.LogInformation("Lấy chi tiết sách {id} thành công !", id);
            return new { status = HttpStatusCode.OK, message = "Lấy chi tiết sách thành công !", data = bookDetailDto };
        }

        public async Task<object> UpdateBookDetail(int id, BookDetailDto bookDetailDto , IFormFile image)
        {
            var bookDetail = await _bookDetailRepository.GetByIdAsync(id);
            if (bookDetail == null)
            {
                return new { status = HttpStatusCode.NotFound, message = "Không tìm thấy chi tiết sách" };
            }
            try
            {
                var isExist = await _bookDetailRepository.GetByIdAsync(id);
                if (isExist == null)
                {
                    return new { status = HttpStatusCode.NotFound, message = "Không tìm thấy chi tiết sách" };
                }
                isExist.book_id = bookDetailDto.book_id;
                isExist.description = bookDetailDto.description;
                isExist.language = bookDetailDto.language;
                isExist.number_of_page = bookDetailDto.number_of_page;
                isExist.price = bookDetailDto.price;
                isExist.file_demo_url = bookDetailDto.file_demo_url;
                isExist.create_at = DateTime.Now;

                if (image != null)
                {
                    var uploadResult = await _cloudService.UploadImageAsync(image);
                    isExist.image_url = uploadResult;
                }

                await _bookDetailRepository.UpdateAsync(isExist);
                _logger.LogInformation("Cập nhật chi tiết sách {id} thành công !", id);
                return new { status = HttpStatusCode.OK, message = "Cập nhật chi tiết sách thành công !" };
            }
            catch(Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật chi tiết sách : {message}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi cập nhật chi tiết sách : " + ex.Message
                };
            }
        }
    }
}
