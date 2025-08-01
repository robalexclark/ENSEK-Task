using CsvHelper.Configuration;

namespace MeterReadingsApi.Models
{
    public sealed class MeterReadingCsvMap : ClassMap<MeterReadingCsvRecord>
    {
        public MeterReadingCsvMap()
        {
            Map(m => m.AccountId).Name("AccountId");
            Map(m => m.MeterReadingDateTime).Name("MeterReadingDateTime").TypeConverterOption.Format("dd/MM/yyyy HH:mm");
            Map(m => m.MeterReadValue).Name("MeterReadValue");
        }
    }
}
