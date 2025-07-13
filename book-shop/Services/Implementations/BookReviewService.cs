using book_shop.Dto;
using book_shop.Helpers.UserHelper;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using System.ComponentModel.Design;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class BookReviewService : IBookReviewService
    {
        private readonly IBookReviewRepository _bookReviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<BookReviewService> _logger;
        private readonly UserHelper _userHelper;

        public BookReviewService(IBookReviewRepository bookReviewRepository, ILogger<BookReviewService> logger, IUserRepository userRepository, IBookRepository bookRepository, UserHelper userHelper)
        {
            _bookReviewRepository = bookReviewRepository;
            _logger = logger;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _userHelper = userHelper;
        }

        public async Task<object> AddReview(BookReviewCreateDto bookReview)
        {
            try
            {
                var userId = _userHelper.GetCurrentUserId();
                await _bookReviewRepository.AddAsync(new BookReview
                {
                    book_id = bookReview.book_id,
                    user_id = userId,
                    rating = bookReview.rating,
                    content = bookReview.content,
                    created_at = DateTime.UtcNow
                });
                _logger.LogInformation("Đánh giá sách đã được thêm thành công: {BookReview}", bookReview);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đánh giá sách đã được thêm thành công",
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Lỗi khi thêm đánh giá sách: {Message}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi thêm đánh giá sách: " + ex.Message,
                };
            }
        }

        public async Task<object> DeleteReview(int id)
        {
            try
            {
                var review = await _bookReviewRepository.GetByIdAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Không tìm thấy đánh giá sách với ID: {Id}", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy đánh giá sách với ID: " + id,
                    };
                }
                await _bookReviewRepository.DeleteAsync(id);
                _logger.LogInformation("Đánh giá sách với ID {Id} đã được xóa thành công", id);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đánh giá sách đã được xóa thành công",
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Lỗi khi xóa đánh giá sách: {Message}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi xóa đánh giá sách: " + ex.Message,
                };
            }
        }

        public async Task<object> GetAllReviews()
        {
            try
            {
                var reviews = await _bookReviewRepository.GetAllAsync();
                if (reviews == null || !reviews.Any())
                {
                    _logger.LogInformation("Không có đánh giá sách nào được tìm thấy.");
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không có đánh giá sách nào được tìm thấy.",
                    };
                }
                var reviewDtos = reviews.Select(r => new BookReviewResponseDto
                {
                    book_review_id = r.book_review_id,
                    book_id = r.book_id,
                    user_id = r.user_id,
                    Book = new BookResponseDto
                    {
                        book_id = r.book.book_id,
                        title = r.book.title,
                        image_url = r.book.image_url,
                        description = r.book.bookDetail?.description,
                        price = r.book.price,
                        category_id = r.book.category_id
                    },
                    User = new UserRespone
                    {
                        user_id = r.user.user_id,
                        first_name = r.user.first_name,
                        last_name = r.user.last_name,
                        email = r.user.email,
                        full_name = r.user.first_name + " " + r.user.last_name,
                        profile_image = r.user.profile_image,
                    },
                    rating = r.rating,
                    content = r.content,
                    created_at = r.created_at
                }).ToList();
                _logger.LogInformation("Đã lấy tất cả đánh giá sách thành công.");
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đã lấy tất cả đánh giá sách thành công.",
                    data = reviewDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy tất cả đánh giá sách: {Message}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi lấy tất cả đánh giá sách: " + ex.Message,
                };
            }
        }

        public async Task<object> GetAverageRatingByBookId(int bookId)
        {
            try
            {
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với ID: {BookId}", bookId);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy sách với ID: " + bookId,
                    };
                }
                var averageRating = await _bookReviewRepository.GetAverageRatingByBookId(bookId);
                _logger.LogInformation("Đã lấy đánh giá trung bình của sách với ID {BookId} thành công", bookId);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đã lấy đánh giá trung bình của sách thành công",
                    data = new
                    {
                        book_id = bookId,
                        average_rating = averageRating
                    }
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Lỗi khi lấy đánh giá trung bình của sách: {Message}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi lấy đánh giá trung bình của sách: " + ex.Message,
                };
            }
        }

        public async Task<object> GetReview(int id)
        {
            try
            {
                var review = await _bookReviewRepository.GetByIdAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Không tìm thấy đánh giá sách với ID: {Id}", id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy đánh giá sách với ID: " + id,
                    };
                }
                var userRespone = await _userRepository.GetByIdAsync(review.user_id);
                var reviewDto = new BookReviewResponseDto
                {
                    book_review_id = review.book_review_id,
                    book_id = review.book_id,
                    user_id = review.user_id,
                    User = new UserRespone
                    {
                        user_id = userRespone.user_id,
                        first_name = userRespone.first_name,
                        email = userRespone.email,
                        full_name = userRespone.first_name + " " + userRespone.last_name,
                        profile_image = userRespone.profile_image,
                    },
                    rating = review.rating,
                    content = review.content,
                    created_at = review.created_at
                };
                _logger.LogInformation("Đã lấy đánh giá sách với ID {Id} thành công", id);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đã lấy đánh giá sách thành công",
                    data = reviewDto
                };

            }
            catch(Exception ex)
            {
                _logger.LogError("Lỗi khi lấy đánh giá sách: {Message}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi lấy đánh giá sách: " + ex.Message,
                };
            }
        }

        public async Task<object> GetReviewCountByBookId(int bookId)
        {
            try
            {
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với ID: {BookId}", bookId);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy sách với ID: " + bookId,
                    };
                }
                var reviewCount = await _bookReviewRepository.GetReviewCountByBookIdAsync(bookId);
                _logger.LogInformation("Đã lấy số lượng đánh giá sách với ID {BookId} thành công", bookId);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đã lấy số lượng đánh giá sách thành công",
                    data = reviewCount
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Lỗi khi lấy số lượng đánh giá sách: {Message}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi lấy số lượng đánh giá sách: " + ex.Message,
                };
            }
        }

        public async Task<object> GetReviewsByBookId(int bookId)
        {
            try
            {
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với ID: {BookId}", bookId);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy sách với ID: " + bookId,
                    };
                }
                var reviews = await _bookReviewRepository.GetReviewsByBookIdAsync(bookId);
                if (reviews == null || !reviews.Any())
                {
                    _logger.LogInformation("Không có đánh giá nào cho sách với ID: {BookId}", bookId);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không có đánh giá nào cho sách với ID: " + bookId,
                    };
                }
                var reviewDtos = reviews.Select(r => new BookReviewResponseDto
                {
                    book_review_id = r.book_review_id,
                    Book = new BookResponseDto
                    {
                        book_id = book.book_id,
                        title =  book.title,
                        image_url = book.image_url,
                        price = book.price,
                        category_id = book.category_id
                    },
                    user_id = r.user_id,
                    User = new UserRespone
                    {
                        user_id = r.user.user_id,
                        first_name = r.user.first_name,
                        last_name = r.user.last_name,
                        email = r.user.email,
                        full_name = r.user.first_name + " " + r.user.last_name,
                        profile_image = r.user.profile_image,
                    },
                    rating = r.rating,
                    content = r.content,
                    created_at = r.created_at
                }).ToList();
                _logger.LogInformation("Đã lấy đánh giá sách với ID {BookId} thành công", bookId);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đã lấy đánh giá sách thành công",
                    data = reviewDtos
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy đánh giá sách theo ID: {BookId}", bookId);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi lấy đánh giá sách theo ID: " + ex.Message,
                };
            }
        }

        public async Task<object> GetReviewsByUserId(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng với ID: {UserId}", userId);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy người dùng với ID: " + userId,
                    };
                }
                var reviews = await _bookReviewRepository.GetReviewsByUserIdAsync(userId);
                if (reviews == null || !reviews.Any())
                {
                    _logger.LogInformation("Không có đánh giá nào cho người dùng với ID: {UserId}", userId);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không có đánh giá nào cho người dùng với ID: " + userId,
                    };
                }
                var reviewDtos = reviews.Select(r => new BookReviewResponseDto
                {
                    book_review_id = r.book_review_id,
                    book_id = r.book_id,
                    Book = new BookResponseDto
                    {
                        book_id = r.book_id,
                        title = r.book.title,
                        image_url = r.book.image_url,
                        description = r.book.bookDetail.description,
                        price = r.book.price,
                        category_id = r.book.category_id
                    },
                    user_id = r.user_id,
                    User = new UserRespone
                    {
                        user_id = user.user_id,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        email = user.email,
                        full_name = user.first_name + " " + user.last_name,
                        profile_image = user.profile_image,
                    },
                    rating = r.rating,
                    content = r.content,
                    created_at = r.created_at
                }).ToList();
                _logger.LogInformation("Đã lấy đánh giá sách theo người dùng với ID {UserId} thành công", userId);
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Đã lấy đánh giá sách theo người dùng thành công",
                    data = reviewDtos
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(userId, "Lỗi khi lấy đánh giá sách theo người dùng: {UserId}", userId);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi lấy đánh giá sách theo người dùng: " + ex.Message,
                };
            }
        }

        public async Task<object> UpdateReview(BookReviewUpdateDto bookReview)
        {
            try
            {
                var existingReview = await _bookReviewRepository.GetByIdAsync(bookReview.book_review_id);
                if (existingReview == null)
                {
                    _logger.LogWarning("Không tìm thấy đánh giá sách với ID: {Id}", bookReview.book_review_id);
                    return new
                    {
                        status = HttpStatusCode.NotFound,
                        msg = "Không tìm thấy đánh giá sách với ID: " + bookReview.book_review_id,
                    };
                }
                else
                {
                    existingReview.book_id = bookReview.book_id;
                    existingReview.user_id = bookReview.user_id;
                    existingReview.rating = bookReview.rating;
                    existingReview.content = bookReview.content;
                    existingReview.created_at = DateTime.UtcNow;
                    await _bookReviewRepository.UpdateAsync(existingReview);
                    _logger.LogInformation("Đánh giá sách với ID {Id} đã được cập nhật thành công", bookReview.book_review_id);
                    return new
                    {
                        status = HttpStatusCode.OK,
                        msg = "Đánh giá sách đã được cập nhật thành công",
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi cập nhật đánh giá sách: {Message}", ex.Message);
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Lỗi khi cập nhật đánh giá sách: " + ex.Message,
                };
            }
        }
    }
}
