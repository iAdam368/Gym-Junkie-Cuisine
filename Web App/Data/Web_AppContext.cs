using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_App.Models;

namespace Web_App.Data
{

    // Add identity to the context 
    public class Web_AppContext : IdentityDbContext
    {
        public Web_AppContext (DbContextOptions<Web_AppContext> options) : base(options)
        {
        }

        public DbSet<Web_App.Models.FoodItem> FoodItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FoodItem>().ToTable("FoodItem");
        }
    }
}
