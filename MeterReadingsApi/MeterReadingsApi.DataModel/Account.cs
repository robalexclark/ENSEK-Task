namespace MeterReadingsApi.DataModel
{
    public class Account
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<MeterReading> MeterReadings { get; set; }
    }
}
