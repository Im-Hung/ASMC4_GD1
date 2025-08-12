using Asm_GD1.Models;
using Microsoft.EntityFrameworkCore;

namespace Asm_GD1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductTopping> ProductToppings { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Size)
                .WithMany()
                .HasForeignKey(p => p.SizeID);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Topping)
                .WithMany()
                .HasForeignKey(p => p.ToppingID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
