using book_shop.Data;
using book_shop.Dto;
using book_shop.Services.Implementations;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Controllers
{
    [Route("api/v1/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("get-books")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("get-book")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost("create-book")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBook([FromForm] AddBookDto book)
        {
            if (book == null)
            {
                return BadRequest("Thông tin chưa đầy đủ");
            }
            var result = await _bookService.AddBook(book);
            return Ok(result);
        }

        [HttpDelete("delete-book")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPut("update-book")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] UpdateBookDto book)
        {
            if (book == null)
            {
                return BadRequest("Thông tin chưa đầy đủ");
            }
            var result = await _bookService.UpdateBookAsync(id, book);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("get-books-by-category")]
        public async Task<IActionResult> GetBooksByCategoryId(int categoryId)
        {
            var books = await _bookService.GetBooksByCategoryIdAsync(categoryId);
            if (books == null)
            {
                return NotFound();
            }
            return Ok(books);
        }
        //TODO : get books by authorId
        [HttpGet("get-books-by-publisher")]
        public async Task<IActionResult> GetBooksByPublisher(string publisher)
        {
            var books = await _bookService.GetBooksByPublisherAsync(publisher);
            if (books == null)
            {
                return NotFound();
            }
            return Ok(books);
        }

        [HttpGet("get-books-by-title")]
        public async Task<IActionResult> GetBooksByTitle(string title)
        {
            var books = await _bookService.GetBooksByTitleAsync(title);
            if (books == null)
            {
                return NotFound();
            }
            return Ok(books);
        }

        [HttpGet("get-books-by-author")]
        public async Task<IActionResult> GetBooksByAuthorId(int authorId)
        {
            var books = await _bookService.GetBooksByAuthorIdAsync(authorId);
            if (books == null)
            {
                return NotFound();
            }
            return Ok(books);
        }

        [HttpGet("get-top-products")]
        public async Task<IActionResult> GetTopProducts(DateTime? startDate, DateTime? endDate)
        {
            var topProducts = await _bookService.GetTopProductsAsync(startDate, endDate);
            if (topProducts == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm nào." });
            }
            return Ok(topProducts);
        }
    }
}
