using book_shop.Commons.CacheKey;
using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<AuthorService> _logger;
        private readonly ICloudService _cloudService;
        //private readonly IRedisCacheService _cache;
        public AuthorService(IAuthorRepository authorRepository, ILogger<AuthorService> logger, ICloudService cloudService)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _cloudService = cloudService;
            //_cache = cache;
        }

        public async Task<object> GetAuthorByNationally(string nationally)
        {
            try
            {
                var author = await _authorRepository.GetAuthorByNationally(nationally);
                if (author == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Tác giả không tồn tại"
                    };
                }
                _logger.LogInformation("Lấy thông tin tác giả theo quốc gia thành công");
                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Lấy thông tin tác giả theo quốc gia thành công",
                    data = author
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi lấy tác giá theo quốc gia {national}", nationally);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi xảy ra khi lấy tác giá theo quốc gia" + ex.Message
                };
            }
        }

        public async Task<object> GetAuthorById(int id)
        {
            try
            {
                //var cached = await _cache.GetAsync<Author>(AuthorCacheKeys.AuthorById(id));
                //if (cached != null)
                //{
                //    _logger.LogInformation("Lấy thông tin tác giả từ cache thành công");
                //    return new
                //    {
                //        status = HttpStatusCode.OK,
                //        message = "Lấy thông tin tác giả từ cache thành công",
                //        data = cached
                //    };
                //}

                var author = await _authorRepository.GetByIdAsync(id);
                if (author == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Tác giả không tồn tại"
                    };
                }
                _logger.LogInformation("Lấy thông tin tác giả theo id thành công");

                //await _cache.SetAsync(AuthorCacheKeys._authorList, author, TimeSpan.FromMinutes(30));
                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Lấy thông tin tác giả theo id thành công",
                    data = author
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi lấy tác giá theo id {id}", id);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi xảy ra khi lấy tác giá theo id" + ex.Message
                };
            }
        }

        public async Task<object> CreateAuthor(AuthorDto author)
        {
            try
            {
                if (author == null)
                {
                    return new
                    {
                        status = HttpStatusCode.BadRequest,
                        message = "Thông tin tác giả không hợp lệ"
                    };
                }
                var newAuthor = new Author
                {
                    name = author.name,
                    nationally = author.nationally,
                    bio = author.bio,
                    dob = author.dob,
                };
                await _authorRepository.AddAsync(newAuthor);
                if (author.image_url != null && author.image_url.Length > 0)
                {
                    var imageUrl = await _cloudService.UploadImageAsync(author.image_url);
                    newAuthor.image_url = imageUrl;
                    await _authorRepository.UpdateAsync(newAuthor);
                }
                else
                {
                    newAuthor.image_url = "https://res.cloudinary.com/ddcomkqut/image/upload/v1751708914/jhdvzedy1hueackj5lfy.jpg";
                    await _authorRepository.UpdateAsync(newAuthor);
                }
                _logger.LogInformation("Thêm tác giả {author} thành công", author.name);

                //await _cache.RemoveAsync(AuthorCacheKeys._authorList);
                //await _cache.RemoveAsync(AuthorCacheKeys.AuthorById(author.author_id));

                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Thêm tác giả thành công",
                    data = newAuthor
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi thêm tác giả {author}", author);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi xảy ra khi thêm tác giả" + ex.Message
                };
            }
        }

        public async Task<object> UpdateAuthor(int id, UpdateAuthorDto author)
        {
            try
            {
                var isExistingAuthor = await _authorRepository.GetByIdAsync(id);
                if (isExistingAuthor == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Tác giả không tồn tại"
                    };
                }

                if (!string.IsNullOrEmpty(author.name))
                {
                    isExistingAuthor.name = author.name;
                }
                if (author.dob != null) isExistingAuthor.dob = author.dob.Value;
                if (!string.IsNullOrEmpty(author.nationally))
                {
                    isExistingAuthor.nationally = author.nationally;
                }
                if (!string.IsNullOrEmpty(author.bio))
                {
                    isExistingAuthor.bio = author.bio;
                }
                if (author.image_url != null && author.image_url.Length > 0)
                {
                    var imageUrl = await _cloudService.UploadImageAsync(author.image_url);
                    isExistingAuthor.image_url = imageUrl;
                }

                await _authorRepository.UpdateAsync(isExistingAuthor);
                _logger.LogInformation("Cập nhật tác giả {author} thành công", author.name);

                //await _cache.RemoveAsync(AuthorCacheKeys._authorList);

                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Cập nhật tác giả thành công",
                    data = isExistingAuthor
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi cập nhật tác giả {author}", author);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi xảy ra khi cập nhật tác giả" + ex.Message
                };
            }
        }

        public async Task<object> DeleteAuthor(int id)
        {
            try
            {
                var isExistingAuthor = await _authorRepository.GetByIdAsync(id);
                if (isExistingAuthor == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Tác giả không tồn tại"
                    };
                }
                await _authorRepository.DeleteAsync(id);
                _logger.LogInformation("Xóa tác giả {id} thành công", id);

                //await _cache.RemoveAsync(AuthorCacheKeys._authorList);

                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Xóa tác giả thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi xóa tác giả {id}", id);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi xảy ra khi xóa tác giả" + ex.Message
                };
            }
        }

        public async Task<object> GetAuthors()
        {
            _logger.LogInformation("Lấy danh sách tác giả thành công");
            var result = await _authorRepository.GetAllAsync();
            return new
            {
                status = HttpStatusCode.OK,
                message = "Lấy danh sách tác giả thành công",
                data = result
            };
        }

        public async Task<object> GetAuthorByBook(int bookId)
        {
            try
            {
                var author = await _authorRepository.GetAuthorsByBookId(bookId);
                if (author == null)
                {
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        message = "Không tìm thấy sách của tác giả này."
                    };
                }
                return new
                {
                    status = HttpStatusCode.OK,
                    message = "Lấy sách của tác giả thành công.",
                    data = author
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi lấy sách của tác giả {bookId}", bookId);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    message = "Lỗi xảy ra khi lấy sách của tác giả" + ex.Message
                };
            }
        }
    }
}
