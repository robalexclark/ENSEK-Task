using MeterReadingsApi.DataModel;

namespace MeterReadingsApi.Repositories
{
    public interface IMeterReadingsRepository
    {
        IEnumerable<Account> GetAccounts();
        Task AddMeterReadingsAsync(IEnumerable<MeterReading> readings);
        bool AccountExists(int accountId);
        bool ReadingExists(int accountId, DateTime dateTime);
        bool HasNewerReading(int accountId, DateTime dateTime);
        Task<IEnumerable<MeterReading>> GetReadingsByAccountAsync(int accountId);
        void EnsureSeedData();
    }
}