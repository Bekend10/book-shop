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
        public DbSet<Author> Authors { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Method> Methods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BookReview> BookReviews { get; set; }
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

            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.user_id);

            modelBuilder.Entity<User>()
                .HasMany(u => u.BookReview)
                .WithOne(br => br.user)
                .HasForeignKey(br => br.user_id)
                .OnDelete(DeleteBehavior.Cascade)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<Book>()
                .HasMany(b => b.bookReviews)
                .WithOne(br => br.book)
                .HasForeignKey(br => br.book_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookDetail>()
                .HasKey(bd => bd.detail_id);

            modelBuilder.Entity<BookDetail>()
                .Property(bd => bd.detail_id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Book>()
               .HasMany(b => b.authors)
               .WithMany(a => a.books)
               .UsingEntity(j => j.ToTable("BookAuthors"));

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.cart_detail)
                .WithOne(cd => cd.cart)
                .HasForeignKey(cd => cd.cart_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartDetail>()
                .HasKey(bd => bd.cart_detail_id);

            modelBuilder.Entity<CartDetail>()
                .Property(cd => cd.cart_detail_id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.book)
                .WithMany(b => b.cartDetails)
                .HasForeignKey(cd => cd.book_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Method>()
                .HasMany(m => m.orders)
                .WithOne(o => o.method)
                .HasForeignKey(o => o.method_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Method>()
                .HasMany(m => m.payments)
                .WithOne(p => p.method)
                .HasForeignKey(p => p.method_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Order)
                .HasForeignKey(o => o.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.orderDetail)
                .WithOne(od => od.order)
                .HasForeignKey<OrderDetail>(od => od.order_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.order)
                .HasForeignKey<Payment>(p => p.order_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.book)
                .WithMany(b => b.orderDetail)
                .HasForeignKey(od => od.book_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.method)
                .WithMany(m => m.payments)
                .HasForeignKey(p => p.method_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .Property(p => p.payment_status)
                .HasConversion<string>();


            base.OnModelCreating(modelBuilder);
        }
    }
}
