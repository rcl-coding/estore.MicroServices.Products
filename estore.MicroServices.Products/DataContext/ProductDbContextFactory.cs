#nullable disable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace estore.MicroServices.Products.DataContext
{
    public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
    {
        public ProductDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings:Database"));

            return new ProductDbContext(optionsBuilder.Options);
        }
    }
}
