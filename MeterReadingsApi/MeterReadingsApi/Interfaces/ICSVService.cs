using MeterReadingsApi.Models;

namespace MeterReadingsApi.Interfaces
{
    public interface ICSVService
    {
        Task<IEnumerable<MeterReadingCsvRecord>> ReadMeterReadingsAsync(Stream stream);
    }
}
