using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeterReadingsApi.Services;
using Xunit;

public class CsvServiceTests
{
    [Fact]
    public async Task ReadMeterReadingsAsync_ReadsRecords()
    {
        const string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                           "1234,01/01/2024 00:00,01234\n" +
                           "2345,01/01/2024 01:00,56789\n";
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var service = new CsvService();

        var records = await service.ReadMeterReadingsAsync(stream);

        Assert.Equal(2, records.Count());
        Assert.Equal(1234, records.First().AccountId);
        Assert.Equal("01234", records.First().MeterReadValue);
    }
}
