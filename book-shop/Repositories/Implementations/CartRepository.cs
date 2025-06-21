using book_shop.Data;
using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Cart entity)
        {
            await _context.Carts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _context.Carts.ToListAsync();
        }

        public async Task<Cart> GetByIdAsync(int id)
        {
            return await _context.Carts.FirstOrDefaultAsync(_ => _.cart_id == id);
        }

        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.cart_detail)
                .FirstOrDefaultAsync(c => c.user_id == userId);

            if (cart == null)
                return null;

            var cartDto = new Cart
            {
                cart_id = cart.cart_id,
                user_id = cart.user_id,
                total_amount = cart.total_amount,              
            };

            return cartDto;
        }
        public async Task UpdateAsync(Cart entity)
        {
            var cart = _context.Carts.FirstOrDefault(_ => _.cart_id == entity.cart_id);
            if (cart != null)
            {
                cart.cart_id = entity.cart_id ;
                cart.user_id = entity.user_id;
                cart.total_amount = entity.total_amount;
                _context.CartDetails.RemoveRange(cart.cart_detail);
                cart.cart_detail = entity.cart_detail; _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}
