using System;
using System.Linq;
using System.Threading.Tasks;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class MeterReadingsRepositoryTests
{
    private static MeterReadingsContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MeterReadingsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new MeterReadingsContext(options);
    }

    [Fact]
    public async Task GetReadingsByAccountAsync_ReturnsOrderedResults()
    {
        using var context = CreateContext();
        context.Accounts.Add(new Account { AccountId = 1, FirstName = "A", LastName = "B" });
        context.MeterReadings.AddRange(
            new MeterReading { AccountId = 1, MeterReadingDateTime = new DateTime(2024,1,2), MeterReadValue = 1 },
            new MeterReading { AccountId = 1, MeterReadingDateTime = new DateTime(2024,1,1), MeterReadValue = 2 }
        );
        context.SaveChanges();

        var repo = new MeterReadingsRepository(context);
        var result = await repo.GetReadingsByAccountAsync(1);

        Assert.Equal(2, result.Count());
        Assert.True(result.First().MeterReadingDateTime < result.Last().MeterReadingDateTime);
    }
}
