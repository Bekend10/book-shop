using book_shop.Models;
using Microsoft.EntityFrameworkCore;

namespace book_shop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Account)
                .WithOne(a => a.user)
                .HasForeignKey<Account>(a => a.user_id);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.role)
                .WithMany(r => r.account)
                .HasForeignKey(a => a.role_id)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
