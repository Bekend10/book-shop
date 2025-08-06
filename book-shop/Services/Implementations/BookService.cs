using book_shop.Commons.CacheKey;
using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace book_shop.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookDetailRepository _bookDetailRepository;
        private readonly ILogger<BookService> _logger;
        private readonly ICloudService _cloudService;
        private readonly IRedisCacheService _cache;
        public BookService(IBookRepository bookService, ILogger<BookService> logger, ICloudService cloudService, IBookDetailRepository bookDetailRepository, IAuthorRepository authorRepository, ICategoryRepository categoryRepository, IRedisCacheService cache)
        {
            _bookRepository = bookService;
            _logger = logger;
            _cloudService = cloudService;
            _bookDetailRepository = bookDetailRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _cache = cache;
        }

        public async Task<object> AddBook([FromForm] AddBookDto book)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(book.category_id);
                if (existingCategory == null)
                {
                    _logger.LogError("Không tìm thấy thể loại có id {categoryId} !", book.category_id);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy thể loại !" };
                }

                var existingAuthor = await _authorRepository.GetByIdAsync(book.author_id);
                if (existingAuthor == null)
                {
                    _logger.LogError("Không tìm thấy tác giả có id {authorId} !", book.author_id);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy tác giả !" };
                }

                var newBook = new Book
                {
                    title = book.title,
                    author_id = book.author_id,
                    category_id = book.category_id,
                    publisher = book.publisher,
                    publisher_year = book.publisher_year,
                    price = book.price,
                    price_origin = book.price_origin,
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
                await _cache.RemoveAsync(BookCacheKeys._bookList);
                await _cache.RemoveAsync(BookCacheKeys.BookByCategory(book.category_id));
                await _cache.RemoveAsync(BookCacheKeys.BookByAuthor(book.author_id));
                await _cache.RemoveAsync(BookCacheKeys.BookById(newBook.book_id));

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

                await _cache.RemoveAsync(BookCacheKeys._bookList);
                await _cache.RemoveAsync(BookCacheKeys.BookByCategory(isExist.category_id));
                await _cache.RemoveAsync(BookCacheKeys.BookById(id));
                await _cache.RemoveAsync(BookCacheKeys.BookByAuthor(isExist.author_id));

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

                var cached = await _cache.GetAsync<IEnumerable<BookResponseDto>>(BookCacheKeys._bookList);
                if (cached != null)
                {
                    _logger.LogInformation("Lấy sách từ cache thành công !");
                    return new
                    {
                        status = HttpStatusCode.OK,
                        msg = "Lấy danh sách sách thành công !",
                        data = cached,
                    };
                }

                var books = await _bookRepository.GetAllAsync();

                var bookDtos = new List<BookResponseDto>();

                foreach (var b in books)
                {
                    var author = await _authorRepository.GetByIdAsync(b.author_id);
                    var category = await _categoryRepository.GetByIdAsync(b.category_id);

                    var ratings = b.bookReviews.Where(r => r.book_id == b.book_id).ToList();

                    var dto = new BookResponseDto
                    {
                        book_id = b.book_id,
                        title = b.title,
                        category = category,
                        author = new Author
                        {
                            author_id = author.author_id,
                            name = author.name,
                            bio = author.bio,
                            dob = author.dob,
                            nationally = author.nationally,
                            image_url = author.image_url
                        },
                        publisher = b.publisher,
                        price = b.price,
                        price_origin = b.price_origin,
                        category_id = b.category_id,
                        image_url = b.image_url,
                        publisher_year = b.publisher_year,
                        is_bn = b.is_bn,
                        quantity = b.quantity,
                        create_at = b.created_at,
                        number_of_page = b.bookDetail.number_of_page,
                        language = b.bookDetail?.language,
                        file_demo_url = b.bookDetail?.file_demo_url,
                        description = b.bookDetail?.description,
                        rating = ratings.Any() ? ratings.Average(r => r.rating) : 0,
                        count_review = ratings.Count
                    };

                    bookDtos.Add(dto);
                }

                await _cache.SetAsync(BookCacheKeys._bookList, bookDtos, TimeSpan.FromMinutes(30));

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

                var cached = await _cache.GetAsync<BookResponseDto>(BookCacheKeys.BookById(id));
                if (cached != null)
                {
                    _logger.LogInformation("Lấy sách từ cache thành công !");
                    return new
                    {
                        status = HttpStatusCode.OK,
                        msg = "Lấy sách thành công !",
                        data = cached,
                    };
                }

                var bookDetail = await _bookDetailRepository.GetBookDetailsByBookIdAsync(id);
                var author = await _authorRepository.GetByIdAsync(book.author_id);
                var category = await _categoryRepository.GetByIdAsync(book.category_id);

                var ratings = book.bookReviews?.Where(r => r.book_id == book.book_id).ToList() ?? new List<BookReview>();
                double averageRating = ratings.Any() ? ratings.Average(r => r.rating) : 0;
                int reviewCount = ratings.Count;

                var bookDto = new BookResponseDto
                {
                    book_id = book.book_id,
                    title = book.title,
                    author = new Author
                    {
                        author_id = author?.author_id ?? 0,
                        name = author?.name,
                        bio = author?.bio,
                        dob = author.dob,
                        nationally = author?.nationally,
                        image_url = author?.image_url
                    },
                    category = category,
                    category_id = book.category_id,
                    publisher = book.publisher,
                    publisher_year = book.publisher_year,
                    price = book.price,
                    price_origin = book.price_origin,
                    quantity = book.quantity,
                    image_url = book.image_url,
                    description = bookDetail?.description,
                    number_of_page = bookDetail?.number_of_page ?? 0,
                    language = bookDetail?.language,
                    file_demo_url = bookDetail?.file_demo_url,
                    rating = averageRating,
                    count_review = reviewCount,
                    is_bn = book.is_bn,
                    create_at = book.created_at
                };

                _logger.LogInformation("Lấy sách {id} thành công !", id);
                await _cache.SetAsync(BookCacheKeys.BookById(id), bookDto, TimeSpan.FromMinutes(30));
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

        public async Task<object> GetBooksByAuthorIdAsync(int authorId)
        {
            try
            {
                var authors = await _authorRepository.GetByIdAsync(authorId);
                if (authors == null)
                {
                    _logger.LogError("Không tìm thấy tác giả có id {authorId} !", authorId);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy tác giả !" };
                }

                var cached = await _cache.GetAsync<BookResponseDto>(BookCacheKeys.BookByAuthor(authorId));
                if (cached != null)
                {
                    _logger.LogInformation("Lấy sách theo tác giả từ cache thành công !");
                    return new
                    {
                        status = HttpStatusCode.OK,
                        msg = "Lấy danh sách sách theo tác giả thành công !",
                        data = cached,
                    };
                }

                var book = await _bookRepository.GetBooksByAuthorIdAsync(authorId);
                if (book == null)
                {
                    _logger.LogError("Không tìm thấy sách theo tác giả có id {authorId} !", authorId);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy sách theo tác giả !" };
                }
                var bookDtos = await Task.WhenAll(book.Select(async b =>
                {
                    var author = await _authorRepository.GetByIdAsync(b.author_id);
                    var bookDetail = await _bookDetailRepository.GetBookDetailsByBookIdAsync(b.book_id);
                    return new BookResponseDto
                    {
                        book_id = b.book_id,
                        title = b.title,
                        author = new Author
                        {
                            author_id = author.author_id,
                            name = author.name,
                            bio = author.bio,
                            dob = author.dob,
                            nationally = author.nationally,
                            image_url = author.image_url
                        },
                        publisher = b.publisher,
                        price = b.price,
                        price_origin = b.price_origin,
                        category_id = b.category_id,
                        image_url = b.image_url,
                        publisher_year = b.publisher_year,
                        is_bn = b.is_bn,
                        quantity = b.quantity,
                        create_at = b.created_at,
                        language = bookDetail.language,
                        number_of_page = bookDetail.number_of_page,
                        file_demo_url = bookDetail.file_demo_url,
                        description = bookDetail.description,
                        rating = b.bookReviews.Where(_ => _.book_id == b.book_id).Average(br => br.rating),
                        count_review = b.bookReviews.Count(br => br.book_id == b.book_id),
                    };
                }));

                _logger.LogInformation("Lấy sách theo tác giả {authorId} thành công !", authorId);
                await _cache.SetAsync(BookCacheKeys.BookByAuthor(authorId), bookDtos, TimeSpan.FromMinutes(30));
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy danh sách sách theo tác giả thành công !",
                    data = bookDtos,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sách theo tác giả !");
                return new
                {
                    status = HttpStatusCode.InternalServerError,
                    msg = "Xảy ra lỗi " + ex.Message,
                };
            }
        }

        public async Task<object> GetBooksByCategoryIdAsync(int categoryId)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    _logger.LogError("Không tìm thấy thể loại có id {categoryId} !", categoryId);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy thể loại !" };
                }

                var cached = _cache.GetAsync<IEnumerable<BookResponseDto>>(BookCacheKeys.BookByCategory(categoryId));
                if (cached != null)
                {
                    _logger.LogInformation("Lấy sách theo thể loại từ cache thành công !");
                    return new
                    {
                        status = HttpStatusCode.OK,
                        msg = "Lấy danh sách sách theo thể loại thành công !",
                        data = cached,
                    };
                }

                var books = await _bookRepository.GetBooksByCategoryIdAsync(categoryId);
                if (books == null || !books.Any())
                {
                    _logger.LogError("Không tìm thấy sách theo thể loại có id {categoryId} !", categoryId);
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy sách theo thể loại !" };
                }

                var bookDtos = await Task.WhenAll(books.Select(async b =>
                {
                    var author = await _authorRepository.GetByIdAsync(b.author_id);
                    var bookDetail = await _bookDetailRepository.GetBookDetailsByBookIdAsync(b.book_id);
                    return new BookResponseDto
                    {
                        book_id = b.book_id,
                        title = b.title,
                        author = new Author
                        {
                            author_id = author.author_id,
                            name = author.name,
                            bio = author.bio,
                            dob = author.dob,
                            nationally = author.nationally,
                            image_url = author.image_url
                        },
                        publisher = b.publisher,
                        price = b.price,
                        price_origin = b.price_origin,
                        category_id = b.category_id,
                        image_url = b.image_url,
                        publisher_year = b.publisher_year,
                        is_bn = b.is_bn,
                        quantity = b.quantity,
                        create_at = b.created_at,
                        language = bookDetail.language,
                        number_of_page = bookDetail.number_of_page,
                        file_demo_url = bookDetail.file_demo_url,
                        description = bookDetail.description,
                        rating = b.bookReviews.Where(_ => _.book_id == b.book_id).Average(br => br.rating),
                        count_review = b.bookReviews.Count(br => br.book_id == b.book_id),
                    };
                }));

                await _cache.SetAsync(BookCacheKeys.BookByCategory(categoryId), bookDtos, TimeSpan.FromMinutes(30));

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

                var bookDtos = await Task.WhenAll(books.Select(async b =>
                {
                    var author = await _authorRepository.GetByIdAsync(b.author_id);
                    var bookDetail = await _bookDetailRepository.GetBookDetailsByBookIdAsync(b.book_id);
                    return new BookResponseDto
                    {
                        book_id = b.book_id,
                        title = b.title,
                        author = new Author
                        {
                            author_id = author.author_id,
                            name = author.name,
                            bio = author.bio,
                            dob = author.dob,
                            nationally = author.nationally,
                            image_url = author.image_url
                        },
                        publisher = b.publisher,
                        price = b.price,
                        price_origin = b.price_origin,
                        category_id = b.category_id,
                        image_url = b.image_url,
                        publisher_year = b.publisher_year,
                        is_bn = b.is_bn,
                        quantity = b.quantity,
                        create_at = b.created_at,
                        language = bookDetail.language,
                        number_of_page = bookDetail.number_of_page,
                        file_demo_url = bookDetail.file_demo_url,
                        description = bookDetail.description,
                        rating = b.bookReviews.Where(_ => _.book_id == b.book_id).Average(br => br.rating),
                        count_review = b.bookReviews.Count(br => br.book_id == b.book_id),
                    };
                }));

                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy danh sách sách theo nhà xuất bản thành công !",
                    data = bookDtos,
                };
            }
            catch (Exception ex)
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
                    author = new Author
                    {
                        author_id = b.author_id,
                        name = b.authors.FirstOrDefault()?.name ?? "Chưa có tác giả",
                        bio = b.authors.FirstOrDefault()?.bio ?? "Chưa có thông tin",
                        dob = b.authors.FirstOrDefault()?.dob ?? DateTime.MinValue,
                        nationally = b.authors.FirstOrDefault()?.nationally ?? "Chưa có quốc tịch",
                        image_url = b.authors.FirstOrDefault()?.image_url ?? "Chưa có ảnh"
                    },
                    price = b.price,
                    price_origin = b.price_origin,
                    category_id = b.category_id,
                    image_url = b.image_url,
                    publisher_year = b.publisher_year,
                    is_bn = b.is_bn,
                    quantity = b.quantity,
                    create_at = b.created_at,
                    language = b.bookDetail.language,
                    number_of_page = b.bookDetail.number_of_page,
                    file_demo_url = b.bookDetail.file_demo_url,
                    description = b.bookDetail.description,
                    rating = b.bookReviews.Where(_ => _.book_id == b.book_id).Average(br => br.rating),
                    count_review = b.bookReviews.Count(br => br.book_id == b.book_id),
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

        public async Task<object> GetTopProductsAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var topProducts = await _bookRepository.GetTopProducts(startDate, endDate);
                if (topProducts == null || !topProducts.Any())
                {
                    _logger.LogError("Không tìm thấy sản phẩm nào !");
                    return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy sản phẩm nào !" };
                }
                foreach (var item in topProducts)
                {
                    var author = await _authorRepository.GetAuthorsByBookId(item.product_id);
                    item.author = author;
                }
                return new
                {
                    status = HttpStatusCode.OK,
                    msg = "Lấy danh sách sản phẩm thành công !",
                    data = topProducts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sản phẩm !");
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

                if (book.category_id.HasValue && book.category_id != existingBook.category_id)
                {
                    var existingCategory = await _categoryRepository.GetByIdAsync(book.category_id.Value);
                    if (existingCategory == null)
                    {
                        _logger.LogError("Không tìm thấy thể loại có id {categoryId} !", book.category_id.Value);
                        return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy thể loại !" };
                    }
                }

                if (book.author_id.HasValue && book.author_id != existingBook.author_id)
                {
                    var existingAuthor = await _authorRepository.GetByIdAsync(book.author_id.Value);
                    if (existingAuthor == null)
                    {
                        _logger.LogError("Không tìm thấy tác giả có id {authorId} !", book.author_id.Value);
                        return new { status = HttpStatusCode.NotFound, msg = "Không tìm thấy tác giả !" };
                    }
                }

                // Cập nhật sách nếu có giá trị
                if (!string.IsNullOrEmpty(book.title)) existingBook.title = book.title;
                if (book.author_id.HasValue) existingBook.author_id = book.author_id.Value;
                if (!string.IsNullOrEmpty(book.publisher)) existingBook.publisher = book.publisher;
                if (book.price.HasValue) existingBook.price = book.price.Value;
                if (book.category_id.HasValue) existingBook.category_id = book.category_id.Value;
                existingBook.publisher_year = book.publisher_year ?? existingBook.publisher_year;
                if (book.quantity.HasValue)
                {
                    existingBook.quantity = book.quantity.Value;
                    book.is_bn = book.quantity > 0 ? 1 : 0;
                }
                if (book.price_origin.HasValue) existingBook.price_origin = book.price_origin.Value;

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

                await _cache.RemoveAsync(BookCacheKeys._bookList);
                await _cache.RemoveAsync(BookCacheKeys.BookByCategory(existingBook.category_id));
                await _cache.RemoveAsync(BookCacheKeys.BookById(id));
                await _cache.RemoveAsync(BookCacheKeys.BookByAuthor(existingBook.author_id));

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
