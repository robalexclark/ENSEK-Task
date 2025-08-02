using MeterReadingsApi.DataModel;
using MeterReadingsApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MeterReadingsApi.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingsRepositoryTests
    {
        private static MeterReadingsRepository CreateRepository()
        {
            DbContextOptions<MeterReadingsContext> options = new DbContextOptionsBuilder<MeterReadingsContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            MeterReadingsContext context = new MeterReadingsContext(options);
            return new MeterReadingsRepository(context);
        }

        [Fact]
        public async Task AddMeterReadingsAsync_PersistsReadings()
        {
            // Arrange
            MeterReadingsRepository repo = CreateRepository();
            repo.EnsureSeedData();
            MeterReading reading = new MeterReading
            {
                AccountId = repo.GetAccounts().First().AccountId,
                MeterReadingDateTime = new DateTime(2024,1,1),
                MeterReadValue = 100
            };

            // Act
            await repo.AddMeterReadingsAsync(new[] { reading });

            // Assert
            Assert.True(repo.ReadingExists(reading.AccountId, reading.MeterReadingDateTime));
            Assert.Single(repo.GetMeterReadingsByAccountId(reading.AccountId));
        }

        [Fact]
        public async Task QueryMethods_ReturnExpectedResults()
        {
            // Arrange
            MeterReadingsRepository repo = CreateRepository();
            repo.EnsureSeedData();
            Account account = repo.GetAccounts().First();
            MeterReading reading = new MeterReading
            {
                AccountId = account.AccountId,
                MeterReadingDateTime = new DateTime(2024,1,1),
                MeterReadValue = 200
            };

            // Act
            await repo.AddMeterReadingsAsync(new[] { reading });

            // Assert
            Assert.True(repo.AccountExists(account.AccountId));
            Assert.False(repo.AccountExists(999));
            Assert.True(repo.ReadingExists(account.AccountId, reading.MeterReadingDateTime));
            Assert.False(repo.ReadingExists(account.AccountId, new DateTime(2023,1,1)));
            Assert.True(repo.HasNewerReading(account.AccountId, new DateTime(2023,12,31)));
            Assert.False(repo.HasNewerReading(account.AccountId, new DateTime(2025,1,1)));
        }
    }
}