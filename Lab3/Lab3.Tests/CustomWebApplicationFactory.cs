using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IProductRepository));
            if (descriptor != null) services.Remove(descriptor);

            services.AddSingleton<IProductRepository>(sp =>
            {
                var repo = new InMemoryProductRepository();
                repo.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m });
                repo.Add(new Product { Id = 2, Name = "Mouse", Price = 29.99m });
                return repo;
            });
        });
    }
}
