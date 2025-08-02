using CsvHelper.Configuration;

namespace MeterReadingsApi.CsvMappers
{
    public sealed class MeterReadingCsvMap : ClassMap<MeterReadingCsvRecord>
    {
        public MeterReadingCsvMap()
        {
            Map(m => m.AccountId).Name("AccountId");
            Map(m => m.MeterReadingDateTime).Name("MeterReadingDateTime");
            Map(m => m.MeterReadValue).Name("MeterReadValue");
        }
    }
}