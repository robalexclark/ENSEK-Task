namespace MeterReadingsApi.DataModel
{
    public class MeterReading
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public int MeterReadValue { get; set; }

        public Account Account { get; set; } = default!;
    }
}