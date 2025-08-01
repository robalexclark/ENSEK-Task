using Scalar.AspNetCore;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Services
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=meterreadings.db";
            builder.Services.AddDbContext<MeterReadingsContext>(options => options.UseSqlite(connectionString));
            builder.Services.AddScoped<IMeterReadingsRepository, MeterReadingsRepository>();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IMeterReadingsRepository>();
                repo.EnsureSeedData();
            }

            // HTTP pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}