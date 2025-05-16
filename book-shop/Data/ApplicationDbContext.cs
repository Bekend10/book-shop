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
        public DbSet<Address> Address { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookDetail> BookDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Account)
                .WithOne(a => a.user)
                .HasForeignKey<Account>(a => a.user_id);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Address)
                .WithOne(a => a.User)
                .HasForeignKey<Address>(a => a.address_id);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.role)
                .WithMany(r => r.account)
                .HasForeignKey(a => a.role_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.book)
                .WithOne(b => b.category)
                .HasForeignKey(b => b.category_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.bookDetail)
                .WithOne(d => d.book)
                .HasForeignKey<BookDetail>(d => d.book_id);

            modelBuilder.Entity<BookDetail>()
                .HasKey(bd => bd.detail_id);

            modelBuilder.Entity<BookDetail>()
                .Property(bd => bd.detail_id)
                .ValueGeneratedOnAdd();



            base.OnModelCreating(modelBuilder);
        }
    }
}
