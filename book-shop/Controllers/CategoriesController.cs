using book_shop.Dto;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _context;
        public CategoriesController(ICategoryService context)
        {
            _context = context;
        }

        [HttpGet("get-categories")]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _context.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("get-category-by-id")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _context.GetCategoryByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("delete-category")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _context.DeleteCategoryAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("create-category")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto category)
        {
            var result = await _context.AddCategoryAsync(category);
            return Ok(result);
        }

        [HttpPut("update-category")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto category)
        {
            var result = await _context.UpdateCategoryAsync(id , category);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
