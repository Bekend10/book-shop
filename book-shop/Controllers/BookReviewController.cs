using book_shop.Dto;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace book_shop.Controllers
{
    [Route("api/v1/bookreviews")]
    [ApiController]
    public class BookReviewController : ControllerBase
    {
        private readonly IBookReviewService _bookReviewService;
        public BookReviewController(IBookReviewService bookReviewService)
        {
            _bookReviewService = bookReviewService;
        }

        [HttpPost("create-review-book")]
        [Authorize]
        public async Task<IActionResult> CreateReviewBook([FromForm] BookReviewCreateDto bookReview)
        {
            if (bookReview == null)
            {
                return BadRequest("Invalid review data.");
            }
            var result = await _bookReviewService.AddReview(bookReview);
            return Ok(result);
        }

        [HttpPut("update-review-book")]
        [Authorize]
        public async Task<IActionResult> UpdateReviewBook([FromForm] BookReviewUpdateDto bookReview)
        {
            if (bookReview == null)
            {
                return BadRequest("Invalid review data.");
            }
            var result = await _bookReviewService.UpdateReview(bookReview);
            return Ok(result);
        }

        [HttpDelete("delete-review-book")]
        [Authorize]
        public async Task<IActionResult> DeleteReviewBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid review ID.");
            }
            var result = await _bookReviewService.DeleteReview(id);
            return Ok(result);
        }

        [HttpGet("get-review-by-id")]
        [Authorize]
        public async Task<IActionResult> GetReview(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid review ID.");
            }
            var result = await _bookReviewService.GetReview(id);
            return Ok(result);
        }

        [HttpGet("get-all-reviews")]
        [Authorize]
        public async Task<IActionResult> GetAllReviews()
        {
            var result = await _bookReviewService.GetAllReviews();
            return Ok(result);
        }

        [HttpGet("get-reviews-by-book-id")]
        [Authorize]
        public async Task<IActionResult> GetReviewsByBookId(int bookId)
        {
            if (bookId <= 0)
            {
                return BadRequest("Invalid book ID.");
            }
            var result = await _bookReviewService.GetReviewsByBookId(bookId);
            return Ok(result);
        }

        [HttpGet("get-reviews-by-user-id")]
        [Authorize]
        public async Task<IActionResult> GetReviewsByUserId(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var result = await _bookReviewService.GetReviewsByUserId(userId);
            return Ok(result);
        }
    }
}
