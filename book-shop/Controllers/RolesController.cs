using book_shop.Dto;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace book_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleservice;

        public RolesController(IRoleService Roleservice)
        {
            _roleservice = Roleservice;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("create-role")]
        public async Task<IActionResult> Create(CreateRoleDto dto)
        {
            var result = await _roleservice.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("update-role")]
        public async Task<IActionResult> UpdateRole(int id , [FromBody] UpdateRoleDto dto)
        {
            var result = await _roleservice.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("delete-role")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _roleservice.GetByIdAsync(id);
            if(role == null) return NotFound();
            return Ok(role);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("get-roles")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _roleservice.GetAllAsync());
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("get-detail")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var role = await _roleservice.GetByIdAsync(id);
            if(role == null)  return NotFound();
            return Ok(role);
        }
    }
}
