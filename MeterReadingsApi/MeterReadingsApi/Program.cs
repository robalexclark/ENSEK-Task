using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Repositories;
using MeterReadingsApi.Services;
using MeterReadingsApi.Validators;
using MeterReadingsApi.CsvMappers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace MeterReadingsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Services
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=meterreadings.db";
            builder.Services.AddDbContext<MeterReadingsContext>(options => options.UseSqlite(connectionString));
            builder.Services.AddScoped<IMeterReadingsRepository, MeterReadingsRepository>();

            builder.Services.AddSingleton<ICSVService, CsvService>();
            builder.Services.AddScoped<IMeterReadingUploadService, MeterReadingUploadService>();
            builder.Services.AddTransient<IValidator<MeterReadingCsvRecord>, MeterReadingCsvRecordValidator>();

            WebApplication app = builder.Build();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                IMeterReadingsRepository repo = scope.ServiceProvider.GetRequiredService<IMeterReadingsRepository>();
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