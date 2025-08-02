using MeterReadingsApi.DataModel;
using MeterReadingsApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MeterReadingsApi.UnitTests
{
    public class MeterReadingsRepositoryTests
    {
        private static MeterReadingsRepository CreateRepository()
        {
            var options = new DbContextOptionsBuilder<MeterReadingsContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new MeterReadingsContext(options);
            return new MeterReadingsRepository(context);
        }

        [Fact]
        public async Task AddMeterReadingsAsync_PersistsReadings()
        {
            var repo = CreateRepository();
            repo.EnsureSeedData();
            var reading = new MeterReading
            {
                AccountId = repo.GetAccounts().First().AccountId,
                MeterReadingDateTime = new DateTime(2024,1,1),
                MeterReadValue = 100
            };

            await repo.AddMeterReadingsAsync(new[] { reading });

            Assert.True(repo.ReadingExists(reading.AccountId, reading.MeterReadingDateTime));
        }

        [Fact]
        public async Task QueryMethods_ReturnExpectedResults()
        {
            var repo = CreateRepository();
            repo.EnsureSeedData();
            var account = repo.GetAccounts().First();
            var reading = new MeterReading
            {
                AccountId = account.AccountId,
                MeterReadingDateTime = new DateTime(2024,1,1),
                MeterReadValue = 200
            };
            await repo.AddMeterReadingsAsync(new[] { reading });

            Assert.True(repo.AccountExists(account.AccountId));
            Assert.False(repo.AccountExists(999));
            Assert.True(repo.ReadingExists(account.AccountId, reading.MeterReadingDateTime));
            Assert.False(repo.ReadingExists(account.AccountId, new DateTime(2023,1,1)));
            Assert.True(repo.HasNewerReading(account.AccountId, new DateTime(2023,12,31)));
            Assert.False(repo.HasNewerReading(account.AccountId, new DateTime(2025,1,1)));
        }
    }
}
