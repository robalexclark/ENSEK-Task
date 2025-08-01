using CsvHelper;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.Interfaces;
using System.Globalization;
using System.Text;

namespace MeterReadingsApi.Services
{
    public class CsvService : ICSVService
    {
        public async Task<IEnumerable<MeterReadingCsvRecord>> ReadMeterReadingsAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<MeterReadingCsvMap>();
            var records = new List<MeterReadingCsvRecord>();

            await foreach (var record in csv.GetRecordsAsync<MeterReadingCsvRecord>())
            {
                records.Add(record);
            }

            return records;
        }
    }
}