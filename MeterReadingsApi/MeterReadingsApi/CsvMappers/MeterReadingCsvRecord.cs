namespace MeterReadingsApi.CsvMappers
{
    public class MeterReadingCsvRecord
    {
        public int AccountId { get; set; }
        public string MeterReadingDateTime { get; set; } = string.Empty;
        public string MeterReadValue { get; set; } = string.Empty;
    }
}