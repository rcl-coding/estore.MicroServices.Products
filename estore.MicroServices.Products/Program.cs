#nullable disable
using estore.MicroServices.Products.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IConfiguration configuration = null;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
      {
          builder.AddJsonFile("local.settings.json", true, true);
          builder.AddEnvironmentVariables();
          configuration = builder.Build();
      })
     .ConfigureServices(services =>
     {
         services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings:Database")));
     })
    .Build();

host.Run();