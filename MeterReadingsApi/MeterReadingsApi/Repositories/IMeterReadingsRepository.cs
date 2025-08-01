using System.Collections.Generic;
using MeterReadingsApi.DataModel;

namespace MeterReadingsApi.Repositories
{
    public interface IMeterReadingsRepository
    {
        IEnumerable<Account> GetAccounts();
        void EnsureSeedData();
    }
}
