#nullable disable
using estore.MicroServices.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace estore.MicroServices.Products.DataContext
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext()
        {
        }

        public ProductDbContext(DbContextOptions<ProductDbContext> options)
              : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }

    }
}
