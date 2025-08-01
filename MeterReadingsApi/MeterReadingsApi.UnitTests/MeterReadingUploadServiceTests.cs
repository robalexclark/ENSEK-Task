using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Repositories;
using MeterReadingsApi.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

public class MeterReadingUploadServiceTests
{
    private class FakeCsvService : ICSVService
    {
        public IEnumerable<MeterReadingCsvRecord> Records { get; set; } = new List<MeterReadingCsvRecord>();
        public Task<IEnumerable<MeterReadingCsvRecord>> ReadMeterReadingsAsync(Stream stream) => Task.FromResult(Records);
    }

    private class FakeRepository : IMeterReadingsRepository
    {
        public List<MeterReading> Added { get; } = new();
        public bool AccountExistsReturn { get; set; } = true;
        public bool ReadingExists(int accountId, DateTime dateTime) => false;
        public bool HasNewerReading(int accountId, DateTime dateTime) => false;
        public IEnumerable<Account> GetAccounts() => Array.Empty<Account>();
        public Task AddMeterReadingsAsync(IEnumerable<MeterReading> readings)
        {
            Added.AddRange(readings);
            return Task.CompletedTask;
        }
        public bool AccountExists(int accountId) => AccountExistsReturn;
        public void EnsureSeedData() { }
        public Task<IEnumerable<MeterReading>> GetReadingsByAccountAsync(int accountId) => Task.FromResult<IEnumerable<MeterReading>>(Array.Empty<MeterReading>());
    }

    private class FakeValidator : IValidator<MeterReadingCsvRecord>
    {
        public Func<MeterReadingCsvRecord, bool> ShouldPass { get; set; } = _ => true;
        public ValidationResult Validate(MeterReadingCsvRecord instance)
        {
            return ShouldPass(instance)
                ? new ValidationResult()
                : new ValidationResult(new[] { new ValidationFailure("", "") });
        }
        public Task<ValidationResult> ValidateAsync(MeterReadingCsvRecord instance, CancellationToken cancellation = default) => Task.FromResult(Validate(instance));
        public ValidationResult Validate(IValidationContext context) => Validate((MeterReadingCsvRecord)context.InstanceToValidate!);
        public Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = default) => Task.FromResult(Validate(context));
        public IValidatorDescriptor CreateDescriptor() => throw new NotImplementedException();
        public bool CanValidateInstancesOfType(Type type) => true;
    }

    private static IFormFile DummyFile() => new FormFile(new MemoryStream(Array.Empty<byte>()), 0, 0, "file", "file.csv");

    [Fact]
    public async Task UploadAsync_AddsValidReadings()
    {
        var csvService = new FakeCsvService
        {
            Records = new[]
            {
                new MeterReadingCsvRecord { AccountId = 1, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = "01234" },
                new MeterReadingCsvRecord { AccountId = 1, MeterReadingDateTime = DateTime.UtcNow.AddHours(1), MeterReadValue = "12345" }
            }
        };
        var repo = new FakeRepository();
        var validator = new FakeValidator();
        var service = new MeterReadingUploadService(csvService, repo, validator);

        var result = await service.UploadAsync(DummyFile());

        Assert.Equal(2, result.Successful);
        Assert.Equal(2, repo.Added.Count);
    }

    [Fact]
    public async Task UploadAsync_SkipsInvalidReadings()
    {
        var csvService = new FakeCsvService
        {
            Records = new[]
            {
                new MeterReadingCsvRecord { AccountId = 1, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = "01234" },
                new MeterReadingCsvRecord { AccountId = 1, MeterReadingDateTime = DateTime.UtcNow.AddHours(1), MeterReadValue = "12345" }
            }
        };
        var repo = new FakeRepository();
        var validator = new FakeValidator { ShouldPass = r => r.MeterReadValue.StartsWith("0") };
        var service = new MeterReadingUploadService(csvService, repo, validator);

        var result = await service.UploadAsync(DummyFile());

        Assert.Equal(1, result.Successful);
        Assert.Equal(1, result.Failed);
    }
}
