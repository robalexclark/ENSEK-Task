using System.Text;

namespace MeterReadingsApi.Services
{
    using CsvHelper;
    using MeterReadingsApi.Interfaces;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class CsvService: ICSVService
    {
        public IEnumerable<string> ReadCsvFileForMeterReadings(string location)
        {
                //using var reader = new StreamReader(location, Encoding.Default);
                //using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                //csv.Context.RegisterClassMap<MeterReadingsMap>();
                //var records = csv.GetRecords<MeterReadingModel>();
                //return records.ToList();

            return new List<string>();
        }
    }
}