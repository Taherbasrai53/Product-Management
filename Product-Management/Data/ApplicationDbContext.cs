using Microsoft.EntityFrameworkCore;
using Product_Management.Models;

namespace Product_Management.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {            
        }
        public DbSet<Retailer> Retailers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}
