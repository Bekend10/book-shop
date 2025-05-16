using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Implementations;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using cloudinary_service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookDetailRepository _bookDetailRepository;
        private readonly ILogger<BookService> _logger;
        private readonly ICloudService _cloudService;
        public BookService(IBookRepository bookService, ILogger<BookService> logger, ICloudService cloudService, IBookDetailRepository bookDetailRepository)
        {
            _bookRepository = bookService;
            _logger = logger;
            _cloudService = cloudService;
            _bookDetailRepository = bookDetailRepository;
        }

        public async Task<object> AddBook([FromForm] AddBookDto book)
        {
            try
            {
                var newBook = new Book
                {
                    title = book.title,
                    author_id = book.author_id,
                    category_id = book.category_id,
                    publisher = book.publisher,
                    publisher_year = book.publisher_year,
                    price = book.price,
                    quantity = book.quantity,
                    created_at = DateTime.Now,
                    is_bn = book.is_bn,
                };

                if (book.image != null)
                {
                    var uploadResult = await _cloudService.UploadImageAsync(book.image);
                    newBook.image_url = uploadResult;
                }

                await _bookRepository.AddAsync(newBook);

                var bookDetail = new BookDetail
                {
                    book_id = newBook.book_id,
                    description = book.description,
                    language = book.language,
                    number_of_page = book.number_of_page,
                    price = book.price,
                    file_demo_url = book.file_demo_url,
                    create_at = DateTime.Now,
                    publisher_year = book.publisher_year,
                    publisher = book.publisher,
                    quantity = book.quantity,
                    is_bn = book.is_bn,
                };

                await _bookDetailRepository.AddAsync(bookDetail);

                _logger.LogInformation("Thêm sách {title} thành công !", book.title);

                return new { status = HttpStatusCode.Created, message = "Thêm sách thành công !" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sách");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message,
                };
            }
        }


        public async Task<object> DeleteBookAsync(int id)
        {
            try
            {
                var isExist = await _bookRepository.GetByIdAsync(id);
                if (isExist == null)
                {
                    _logger.LogError("Không tìm thấy sách có id {id} !", id);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy sách !" };
                }
                var detail = await _bookDetailRepository.GetBookDetailsByBookIdAsync(id);
                await _bookRepository.DeleteAsync(id);
                await _bookDetailRepository.DeleteAsync(detail.detail_id);
                _logger.LogInformation("Xoá sách {id} thành công !", id);
                return new { status = HttpStatusCode.OK, msg = "Xoá sách thành công !" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xoá sách !");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message,
                };
            }
        }

        public async Task<object> GetAllBooksAsync()
        {
            try
            {
                _logger.LogInformation("Lấy danh sách sách thành công !");
                var books = await _bookRepository.GetAllAsync();
                var bookDtos = books.Select(b => new BookResponseDto
                {
                    book_id = b.book_id,
                    title = b.title,
                    author_id = b.author_id,
                    publisher = b.publisher,
                    price = b.price,
                    category_id = b.category_id,
                    image_url = b.image_url,
                    publisher_year = b.publisher_year,
                    is_bn = b.is_bn,
                    quantity = b.quantity,
                    create_at = b.created_at,

                }).ToList();
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy danh sách sách thành công !",
                    data = bookDtos,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sách !");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message,
                };
            }
        }

        public async Task<object> GetBookByIdAsync(int id)
        {
            try
            {
                var book = await _bookRepository.GetByIdAsync(id);
                if (book == null)
                {
                    _logger.LogError("Không tìm thấy sách có id {id} !", id);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy sách !" };
                }
                var bookDetail = await _bookDetailRepository.GetBookDetailsByBookIdAsync(id);
                var bookDto = new BookResponseDto
                {
                    book_id = book.book_id,
                    title = book.title,
                    author_id = book.author_id,
                    publisher = book.publisher,
                    price = book.price,
                    category_id = book.category_id,
                    image_url = book.image_url,
                    is_bn = book.is_bn,
                    quantity = book.quantity,
                    create_at = bookDetail.create_at,
                    description = bookDetail.description,
                    language = bookDetail.language,
                    number_of_page = bookDetail.number_of_page,
                    file_demo_url = bookDetail.file_demo_url,
                    publisher_year = bookDetail.publisher_year,                   
                };
                _logger.LogInformation("Lấy sách {id} thành công !", id);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy sách thành công !",
                    data = bookDto,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sách !");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message,
                };
            }
        }

        public Task<object> GetBooksByAuthorIdAsync(int authorId)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetBooksByCategoryIdAsync(int categoryId)
        {
            try
            {
                var books = await _bookRepository.GetBooksByCategoryIdAsync(categoryId);
                if (books == null || !books.Any())
                {
                    _logger.LogError("Không tìm thấy sách theo thể loại có id {categoryId} !", categoryId);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy sách theo thể loại !" };
                }
                var bookDtos = books.Select(b => new BookResponseDto
                {
                    book_id = b.book_id,
                    title = b.title,
                    author_id = b.author_id,
                    publisher = b.publisher,
                    price = b.price,
                    category_id = b.category_id,
                    image_url = b.image_url,
                    publisher_year = b.publisher_year,
                    is_bn = b.is_bn,
                    quantity = b.quantity,
                    create_at = b.created_at,
                }).ToList();
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy danh sách sách theo thể loại thành công !",
                    data = bookDtos,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sách theo thể loại !");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message,
                };
            }
        }

        public async Task<object> GetBooksByPublisherAsync(string publisher)
        {
            try
            {
                var books = await _bookRepository.GetBooksByPublisherAsync(publisher);
                if (books == null || !books.Any())
                {
                    _logger.LogError("Không tìm thấy sách theo nhà xuất bản {publisher} !", publisher);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy sách theo nhà xuất bản !" };
                }
                var bookDtos = books.Select(b => new BookResponseDto
                {
                    book_id = b.book_id,
                    title = b.title,
                    author_id = b.author_id,
                    publisher = b.publisher,
                    price = b.price,
                    category_id = b.category_id,
                    image_url = b.image_url,
                    publisher_year = b.publisher_year,
                    is_bn = b.is_bn,
                    quantity = b.quantity,
                    create_at = b.created_at,
                }).ToList();
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy danh sách sách theo nhà xuất bản thành công !",
                    data = bookDtos,
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sách theo nhà xuất bản !");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message,
                };
            }
        }

        public async Task<object> GetBooksByTitleAsync(string title)
        {
            try
            {
                var books = await _bookRepository.GetBooksByTitleAsync(title);
                if (books == null || !books.Any())
                {
                    _logger.LogError("Không tìm thấy sách theo tiêu đề {title} !", title);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy sách theo tiêu đề !" };
                }
                var bookDtos = books.Select(b => new BookResponseDto
                {
                    book_id = b.book_id,
                    title = b.title,
                    author_id = b.author_id,
                    publisher = b.publisher,
                    price = b.price,
                    category_id = b.category_id,
                    image_url = b.image_url,
                    publisher_year = b.publisher_year,
                    is_bn = b.is_bn,
                    quantity = b.quantity,
                    create_at = b.created_at,
                }).ToList();
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy danh sách sách theo tiêu đề thành công !",
                    data = bookDtos,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sách theo tiêu đề !");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message,
                };
            }
        }

        public async Task<object> UpdateBookAsync(int id, [FromForm] UpdateBookDto book)
        {
            try
            {
                var existingBook = await _bookRepository.GetByIdAsync(id);
                if (existingBook == null)
                {
                    _logger.LogError("Không tìm thấy sách có id {id} !", id);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy sách !" };
                }

                var existingBookDetail = await _bookDetailRepository.GetBookDetailsByBookIdAsync(id);
                if (existingBookDetail == null)
                {
                    _logger.LogError("Không tìm thấy chi tiết sách có id {id} !", id);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy chi tiết sách !" };
                }

                // Cập nhật sách nếu có giá trị
                if (!string.IsNullOrEmpty(book.title)) existingBook.title = book.title;
                if (book.author_id.HasValue) existingBook.author_id = book.author_id.Value;
                if (!string.IsNullOrEmpty(book.publisher)) existingBook.publisher = book.publisher;
                if (book.price.HasValue) existingBook.price = book.price.Value;
                if (book.category_id.HasValue) existingBook.category_id = book.category_id.Value;
                existingBook.publisher_year = book.publisher_year ?? existingBook.publisher_year;
                if (book.quantity.HasValue) existingBook.quantity = book.quantity.Value;
                if (book.is_bn.HasValue) existingBook.is_bn = book.is_bn.Value;

                if (book.image != null)
                {
                    var uploadResult = await _cloudService.UploadImageAsync(book.image);
                    existingBook.image_url = uploadResult;
                }

                existingBook.created_at = DateTime.Now;

                if (!string.IsNullOrEmpty(book.description)) existingBookDetail.description = book.description;
                if (!string.IsNullOrEmpty(book.language)) existingBookDetail.language = book.language;
                if (book.number_of_page.HasValue) existingBookDetail.number_of_page = book.number_of_page.Value;
                if (!string.IsNullOrEmpty(book.file_demo_url)) existingBookDetail.file_demo_url = book.file_demo_url;
                if (book.price.HasValue) existingBookDetail.price = book.price.Value;

                existingBookDetail.create_at = DateTime.Now;

                await _bookRepository.UpdateAsync(existingBook);
                await _bookDetailRepository.UpdateAsync(existingBookDetail);

                _logger.LogInformation("Cập nhật sách {id} thành công !", id);
                return new { status = HttpStatusCode.OK, msg = "Cập nhật sách thành công !" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sách !");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message,
                };
            }
        }
    }
}
