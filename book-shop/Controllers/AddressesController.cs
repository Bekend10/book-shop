using book_shop.Dto;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace book_shop.Controllers
{
    [Route("api/v1/addresses")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("get-addresses")]
        public async Task<IActionResult> GetAllAddresses()
        {
            var result = await _addressService.GetAllAddresses();
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("get-address-detail")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var result = await _addressService.GetAddressById(id);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("delete-address")]
        public async Task<IActionResult> DeleteAddressById(int id)
        {
            var result = await _addressService.DeleteAddress(id);
            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [Route("update-address")]
        public async Task<IActionResult> UpdateAddress(int id , UpdateAddressDto dto)
        {
            var result = await _addressService.UpdateAddress(id , dto);
            return Ok(result);
        }
    }
}
