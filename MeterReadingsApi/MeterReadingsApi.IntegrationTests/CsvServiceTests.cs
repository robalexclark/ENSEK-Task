using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.Services;
using System.Text;
using Xunit;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadingsApi.UnitTests
{
    public class CsvServiceTests
    {
        [Fact]
        public async Task ReadMeterReadingsAsync_ParsesRecordsCorrectly()
        {
            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                      "1234,16/05/2019 09:24,00123\n" +
                      "5678,17/05/2019 12:00,00456\n";
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            var service = new CsvService();

            var result = await service.ReadMeterReadingsAsync(stream);
            var list = result.ToList();

            Assert.Equal(2, list.Count);
            Assert.Equal(1234, list[0].AccountId);
            Assert.Equal(new System.DateTime(2019, 5, 16, 9, 24, 0), list[0].MeterReadingDateTime);
            Assert.Equal("00123", list[0].MeterReadValue);
            Assert.Equal(5678, list[1].AccountId);
            Assert.Equal("00456", list[1].MeterReadValue);
        }
    }
}
