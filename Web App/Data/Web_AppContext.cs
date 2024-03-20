using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_App.Models;

namespace Web_App.Data
{

    public class Web_AppContext : IdentityDbContext
    {
        public Web_AppContext (DbContextOptions<Web_AppContext> options) : base(options)
        {
        }

        public DbSet<Web_App.Models.FoodItem> FoodItems { get; set; }

        // Add Basket, BasketItem, CheckoutCustomer, OrderHistory and OrderItem tables to db context
        public DbSet<CheckoutCustomer> CheckoutCustomers { get; set; } = default!;
        public DbSet<Basket> Baskets { get; set; } = default!;
        public DbSet<BasketItem> BasketItems { get; set; } = default!;
        public DbSet<OrderHistory> OrderHistories { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;
        // Add non-database CheckoutItem data to context
        [NotMapped]
        public DbSet<CheckoutItem> CheckoutItems { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FoodItem>().ToTable("FoodItem");

            // Assign composite primary key to BasketItem model
            modelBuilder.Entity<BasketItem>().HasKey(t => new { t.StockID, t.BasketID });
            // Assign composite primary key to OrderItem model
            modelBuilder.Entity<OrderItem>().HasKey(t => new { t.OrderNo, t.StockID });
        }
    }
}
