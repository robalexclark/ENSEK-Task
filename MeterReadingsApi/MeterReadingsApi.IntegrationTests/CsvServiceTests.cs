using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.IO;
using Xunit;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class CsvServiceTests
    {
        [Fact]
        public async Task ReadMeterReadingsAsync_ParsesRecordsCorrectly()
        {
            // Arrange
            string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                      "1234,16/05/2019 09:24,00123\n" +
                      "5678,17/05/2019 12:00,00456\n";

            await using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            CsvService service = new CsvService();

            // Act
            IEnumerable<MeterReadingCsvRecord> result = await service.ReadMeterReadingsAsync(stream);
            List<MeterReadingCsvRecord> list = result.ToList();

            // Assert
            Assert.Equal(2, list.Count);
            Assert.Equal("1234", list[0].AccountId);
            Assert.Equal("16/05/2019 09:24", list[0].MeterReadingDateTime);
            Assert.Equal("00123", list[0].MeterReadValue);
            Assert.Equal("5678", list[1].AccountId);
            Assert.Equal("17/05/2019 12:00", list[1].MeterReadingDateTime);
            Assert.Equal("00456", list[1].MeterReadValue);
        }

        [Fact]
        public async Task ReadMeterReadingsAsync_SkipsBlankRows()
        {
            // Arrange
            string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                         "1234,16/05/2019 09:24,00123\n" +
                         "\n" +
                         "5678,17/05/2019 12:00,00456\n" +
                         "\n";

            await using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            CsvService service = new CsvService();

            // Act
            IEnumerable<MeterReadingCsvRecord> result = await service.ReadMeterReadingsAsync(stream);
            List<MeterReadingCsvRecord> list = result.ToList();

            // Assert
            Assert.Equal(2, list.Count);
            Assert.Equal("1234", list[0].AccountId);
            Assert.Equal("5678", list[1].AccountId);
        }
    }
}