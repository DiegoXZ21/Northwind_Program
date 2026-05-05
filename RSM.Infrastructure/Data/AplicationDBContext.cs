//This is just to try
using Microsoft.EntityFrameworkCore;
using RSM.Domain.Models;

namespace RSM.Infrastructure.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Shipper> Shippers {get; set;}
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<OrderDetails> OrderDetails {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetails>()
            .ToTable("Order Details")
                .HasKey(od => new { od.OrderId, od.ProductId });

            base.OnModelCreating(modelBuilder);
        }
        
    }
}