using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Topping> Toppings { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customize> Customizes { get; set; }
        public DbSet<CustomizeTopping> CustomizeToppings { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OTPCode> OTPCodes { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
             .HasOne(u => u.Customer)
             .WithOne(c => c.User)
             .HasForeignKey<Customer>(c => c.UserId)
             .IsRequired(false);
            modelBuilder.Entity<Customer>()
            .Property(c => c.Wallet)
            .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(C => C.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<CustomizeTopping>()
       .HasKey(ct => new { ct.CustomizeId, ct.ToppingId });

            modelBuilder.Entity<CustomizeTopping>()
                .HasOne(ct => ct.Customize)
                .WithMany(c => c.CustomizeToppings)
                .HasForeignKey(ct => ct.CustomizeId);

            modelBuilder.Entity<CustomizeTopping>()
                .HasOne(ct => ct.Topping)
                .WithMany(t => t.CustomizeToppings)
                .HasForeignKey(ct => ct.ToppingId);

            
            modelBuilder.Entity<Customize>()
                .Property(c => c.Ice)
                .HasConversion<string>();

            modelBuilder.Entity<Customize>()
                .Property(c => c.Sugar)
                .HasConversion<string>();

           /* modelBuilder.Entity<Customize>()
                .Property(c => c.Temperature)
                .HasConversion<string>();*/

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Cart)
                .WithOne(c => c.Customer)
                .HasForeignKey<Cart>(c => c.CustomerId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}



