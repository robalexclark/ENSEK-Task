using MeterReadingsApi.DataModel;
using MeterReadingsApi.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MeterReadingsApi.IntegrationTests;

[ExcludeFromCodeCoverage]
public class TestApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            List<ServiceDescriptor> descriptors = services.Where(d => d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("MeterReadingsContext")).ToList();
            foreach (ServiceDescriptor d in descriptors)
            {
                services.Remove(d);
            }

            services.AddDbContext<MeterReadingsContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Build provider to seed data
            ServiceProvider sp = services.BuildServiceProvider();
            using IServiceScope scope = sp.CreateScope();
            IMeterReadingsRepository repo = scope.ServiceProvider.GetRequiredService<IMeterReadingsRepository>();
            repo.EnsureSeedData();
        });
    }
}