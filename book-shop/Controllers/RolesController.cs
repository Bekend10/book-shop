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
    }
}
