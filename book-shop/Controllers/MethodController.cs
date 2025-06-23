using book_shop.Dto;
using book_shop.Models;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace book_shop.Controllers
{
    [Route("api/v1/method")]
    [ApiController]
    public class MethodController : ControllerBase
    {
        private readonly IMethodService _methodService;
        public MethodController(IMethodService methodService)
        {
            _methodService = methodService;
        }

        [HttpGet("get-all-methods")]
        [Authorize]
        public async Task<IActionResult> GetAllMethods()
        {
            var methods = await _methodService.GetAllMethods();
            return Ok(methods);
        }

        [HttpPost("add-method")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddMethod([FromBody] MethodDto method)
        {
            var createdMethod = await _methodService.AddMethod(method);
            return Ok(createdMethod);
        }

        [HttpGet("get-method-by-id")]
        [Authorize]
        public async Task<IActionResult> GetMethodById(int id)
        {
            var method = await _methodService.GetMethodById(id);
            return Ok(method);
        }

        [HttpPut("update-method")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateMethod(int id, [FromBody] UpdateMethodDto method)
        {
            var updatedMethod = await _methodService.UpdateMethod(id, method);
            return Ok(updatedMethod);
        }

        [HttpDelete("delete-method")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteMethod(int id)
        {
            var deletedMethod = await _methodService.DeleteMethod(id);
            return Ok(deletedMethod);
        }
    }
}
