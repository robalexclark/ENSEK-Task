using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Repositories;
using MeterReadingsApi.Validators;
using FluentValidation.Results;
using System.Diagnostics.CodeAnalysis;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingCsvRecordValidatorTests
    {
        private class FakeRepository : IMeterReadingsRepository
        {
            public bool AccountExistsReturn { get; set; } = true;
            public bool ReadingExistsReturn { get; set; } = false;
            public bool HasNewerReadingReturn { get; set; } = false;

            public IEnumerable<Account> GetAccounts() => Array.Empty<Account>();
            public Task AddMeterReadingsAsync(IEnumerable<MeterReading> readings) => Task.CompletedTask;
            public bool AccountExists(int accountId) => AccountExistsReturn;
            public bool ReadingExists(int accountId, DateTime dateTime) => ReadingExistsReturn;
            public bool HasNewerReading(int accountId, DateTime dateTime) => HasNewerReadingReturn;
            public void EnsureSeedData() { }
        }

        [Fact]
        public void Valid_record_passes_validation()
        {
            FakeRepository repo = new FakeRepository();
            MeterReadingCsvRecordValidator validator = new MeterReadingCsvRecordValidator(repo);
            MeterReadingCsvRecord record = new MeterReadingCsvRecord
            {
                AccountId = 1,
                MeterReadingDateTime = DateTime.UtcNow,
                MeterReadValue = "01234"
            };

            ValidationResult result = validator.Validate(record);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Invalid_meter_read_value_fails_validation()
        {
            FakeRepository repo = new FakeRepository();
            MeterReadingCsvRecordValidator validator = new MeterReadingCsvRecordValidator(repo);
            MeterReadingCsvRecord record = new MeterReadingCsvRecord
            {
                AccountId = 1,
                MeterReadingDateTime = DateTime.UtcNow,
                MeterReadValue = "1234" // only 4 digits
            };

            ValidationResult result = validator.Validate(record);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Duplicate_reading_fails_validation()
        {
            FakeRepository repo = new FakeRepository { ReadingExistsReturn = true };
            MeterReadingCsvRecordValidator validator = new MeterReadingCsvRecordValidator(repo);
            MeterReadingCsvRecord record = new MeterReadingCsvRecord
            {
                AccountId = 1,
                MeterReadingDateTime = DateTime.UtcNow,
                MeterReadValue = "12345"
            };

            ValidationResult result = validator.Validate(record);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Older_reading_fails_validation()
        {
            FakeRepository repo = new FakeRepository { HasNewerReadingReturn = true };
            MeterReadingCsvRecordValidator validator = new MeterReadingCsvRecordValidator(repo);
            MeterReadingCsvRecord record = new MeterReadingCsvRecord
            {
                AccountId = 1,
                MeterReadingDateTime = DateTime.UtcNow,
                MeterReadValue = "12345"
            };

            ValidationResult result = validator.Validate(record);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Unknown_account_fails_validation()
        {
            FakeRepository repo = new FakeRepository { AccountExistsReturn = false };
            MeterReadingCsvRecordValidator validator = new MeterReadingCsvRecordValidator(repo);
            MeterReadingCsvRecord record = new MeterReadingCsvRecord
            {
                AccountId = 1,
                MeterReadingDateTime = DateTime.UtcNow,
                MeterReadValue = "12345"
            };

            ValidationResult result = validator.Validate(record);
            Assert.False(result.IsValid);
        }
    }
}