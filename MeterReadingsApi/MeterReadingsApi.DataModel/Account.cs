namespace MeterReadingsApi.DataModel
{
    public class Account
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<MeterReading> MeterReadings { get; set; } = new();
    }
}