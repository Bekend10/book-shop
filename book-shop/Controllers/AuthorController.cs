using book_shop.Data;
using book_shop.Dto;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Controllers
{
    [Route("api/v1/authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet("get-authors")]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAuthors();
            return Ok(authors);
        }

        [HttpGet("get-author-by-id")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorById(id);
            return Ok(author);
        }

        [HttpGet("get-author-by-nationally")]
        public async Task<IActionResult> GetAuthorByNationally(string nationally)
        {
            var author = await _authorService.GetAuthorByNationally(nationally);
            return Ok(author);
        }

        [HttpPost("create-author")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDto author)
        {
            var createdAuthor = await _authorService.CreateAuthor(author);
            return Ok(createdAuthor);
        }

        [HttpPut("update-author")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorDto author)
        {
            var updatedAuthor = await _authorService.UpdateAuthor(id, author);
            return Ok(updatedAuthor);
        }

        [HttpDelete("delete-author")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var deletedAuthor = await _authorService.DeleteAuthor(id);
            return Ok(deletedAuthor);
        }
    }
}
