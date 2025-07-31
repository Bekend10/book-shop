using book_shop.Dto;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace book_shop.Controllers
{
    [Route("api/v1/carts")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartDetailService)
        {
            _cartService = cartDetailService;
        }

        [HttpGet("get-my-cart")]
        [Authorize]
        public async Task<IActionResult> GetMyCart()
        {
            var cartDetails = await _cartService.GetCartDetailsAsync();
            if (cartDetails == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.NotFound,
                    msg = "Giỏ hàng trống"
                });
            }

            return Ok(cartDetails);
        }

        [HttpPost("add-to-cart")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var result = await _cartService.AddToCartAsync(dto);
            if (result == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.NotFound,
                    msg = "Sách không tồn tại"
                });
            }

            return Ok(result);
        }

        [HttpDelete("remove-from-cart")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int bookId)
        {
            var result = await _cartService.RemoveFromCartAsync(bookId);
            if (result == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.NotFound,
                    msg = "Sách không tồn tại trong giỏ hàng"
                });
            }

            return Ok(result);
        }

        [HttpDelete("clear-cart")]
        [Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCartAsync();
            if (result == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.NotFound,
                    msg = "Giỏ hàng không tồn tại"
                });
            }

            return Ok(result);
        }

        [HttpPut("update-cart-item-quantity")]
        [Authorize]
        public async Task<IActionResult> UpdateCartItemQuantity(UpdateQuantityToCartDto dto)
        {
            var result = await _cartService.UpdateCartItemQuantityAsync(dto);
            if (result == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.NotFound,
                    msg = "Sách không tồn tại trong giỏ hàng"
                });
            }

            return Ok(result);
        }
    }
}
