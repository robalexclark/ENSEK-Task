namespace MeterReadingsApi.CsvMappers
{
    public class MeterReadingCsvRecord
    {
        public string AccountId { get; set; } = string.Empty;
        public string MeterReadingDateTime { get; set; } = string.Empty;
        public string MeterReadValue { get; set; } = string.Empty;
    }
}