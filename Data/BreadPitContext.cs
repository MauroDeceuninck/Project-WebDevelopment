using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
    public class BreadPitContext : DbContext
    {
        public BreadPitContext(DbContextOptions<BreadPitContext> options)
            : base(options)
        {
        }


        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        //public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BreadPit;Trusted_Connection=True;");

        }
    }
}
