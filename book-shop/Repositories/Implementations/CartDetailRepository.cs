    using book_shop.Data;
    using book_shop.Models;
    using book_shop.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    namespace book_shop.Repositories.Implementations
    {
        public class CartDetailRepository : ICartDetailRepository
        {
            private readonly ApplicationDbContext _context;
            public CartDetailRepository(ApplicationDbContext context)
            {
                _context = context;    
            }
            public async Task AddAsync(CartDetail entity)
            {
                 await _context.CartDetails.AddAsync(entity);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(int id)
            {
                var cartDetail = await _context.CartDetails.FindAsync(id);
                if (cartDetail != null)
                {
                    _context.CartDetails.Remove(cartDetail);
                    await _context.SaveChangesAsync();
                }
            }

            public async Task DeleteByCartIdAndBookIdAsync(int cartId, int bookId)
            {
                var cartDetail = await _context.CartDetails
                    .FirstOrDefaultAsync(cd => cd.cart_id == cartId && cd.book_id == bookId);

                if (cartDetail != null)
                {
                    _context.CartDetails.Remove(cartDetail);
                    await _context.SaveChangesAsync();
                }
            }


            public async Task<IEnumerable<CartDetail>> GetAllAsync()
            {
                return await _context.CartDetails.ToListAsync();
            }

            public async Task<CartDetail> GetByCartIdAndBookIdAsync(int cartId, int bookId)
            {
                var cartDetail = await _context.CartDetails
                    .FirstOrDefaultAsync(cd => cd.cart_id == cartId && cd.book_id == bookId);
                return cartDetail;
            }

            public async Task<List<CartDetail>> GetByCartIdAsync(int cartId)
            {
                return await _context.CartDetails
                .Include(cd => cd.book)
                .Where(cd => cd.cart_id == cartId)
                .ToListAsync();
            }

            public async Task<CartDetail> GetByIdAsync(int id)
            {
                return await _context.CartDetails.FirstOrDefaultAsync(cd => cd.cart_detail_id == id);
            }

            public async Task UpdateAsync(CartDetail entity)
            {
                var cartDetail = await _context.CartDetails
                    .FirstOrDefaultAsync(cd => cd.cart_id == entity.cart_id && cd.book_id == entity.book_id);

                if (cartDetail != null)
                {
                    cartDetail.quantity = entity.quantity;
                    _context.CartDetails.Update(cartDetail);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
