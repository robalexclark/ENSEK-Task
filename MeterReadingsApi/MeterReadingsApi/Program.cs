using AutoMapper;
using FluentValidation;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Models;
using MeterReadingsApi.Repositories;
using MeterReadingsApi.Services;
using MeterReadingsApi.Validators;
using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddAutoMapper(typeof(Program));

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=meterreadings.db";
            builder.Services.AddDbContext<MeterReadingsContext>(options => options.UseSqlite(connectionString));
            builder.Services.AddScoped<IMeterReadingsRepository, MeterReadingsRepository>();

            builder.Services.AddSingleton<ICSVService, CsvService>();
            builder.Services.AddScoped<IMeterReadingUploadService, MeterReadingUploadService>();
            builder.Services.AddTransient<IValidator<MeterReadingCsvRecord>, MeterReadingCsvRecordValidator>();
            builder.Services.AddTransient<IValidator<int>, AccountIdValidator>();
            builder.Services.AddTransient<IValidator<MeterReadingUploadRequest>, MeterReadingUploadRequestValidator>();

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

