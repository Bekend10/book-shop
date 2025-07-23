using book_shop.Data;
using book_shop.Dto;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace book_shop.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddAsync(User entity)
        {
            User user = entity;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> CreateNewUser(UserGoogleDto model)
        {
            var newUser = new User
            {
                google_id = model.google_id,
                first_name = model.first_name,
                last_name = model.last_name,
                email = model.email,
                profile_image = model.profile_img,
                phone_number = model.phone_number,
                address_id = model.address_id,
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByGoogleIdAsync(string googleId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.google_id == googleId);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstAsync(x => x.user_id == id);
        }

        public async Task<int> GetCurrentUserIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
                return -1;

            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return -1;

            return userId;
        }

        public Task<User?> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(User entity)
        {
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.user_id == entity.user_id);
            if (existingUser == null)
                throw new KeyNotFoundException("User not found");
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
