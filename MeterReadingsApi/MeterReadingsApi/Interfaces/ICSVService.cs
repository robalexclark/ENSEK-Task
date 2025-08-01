using MeterReadingsApi.CsvMappers;

namespace MeterReadingsApi.Interfaces
{
    public interface ICSVService
    {
        Task<IEnumerable<MeterReadingCsvRecord>> ReadMeterReadingsAsync(Stream stream);
    }
}
