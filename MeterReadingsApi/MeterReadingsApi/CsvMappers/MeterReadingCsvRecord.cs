namespace MeterReadingsApi.CsvMappers
{
    public class MeterReadingCsvRecord
    {
        public string? AccountId { get; set; }
        public string? MeterReadingDateTime { get; set; }
        public string? MeterReadValue { get; set; }
    }
}