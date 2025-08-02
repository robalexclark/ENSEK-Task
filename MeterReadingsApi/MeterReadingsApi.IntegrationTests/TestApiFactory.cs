using System.Linq;
using System.Diagnostics.CodeAnalysis;
using MeterReadingsApi;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MeterReadingsApi.IntegrationTests;

[ExcludeFromCodeCoverage]
public class TestApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d => d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("MeterReadingsContext")).ToList();
            foreach (var d in descriptors)
            {
                services.Remove(d);
            }

            services.AddDbContext<MeterReadingsContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Build provider to seed data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IMeterReadingsRepository>();
            repo.EnsureSeedData();
        });
    }
}
