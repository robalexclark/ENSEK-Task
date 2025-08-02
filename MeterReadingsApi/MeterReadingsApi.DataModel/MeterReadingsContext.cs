using Microsoft.EntityFrameworkCore;

namespace MeterReadingsApi.DataModel
{
    public class MeterReadingsContext : DbContext
    {
        public MeterReadingsContext(DbContextOptions<MeterReadingsContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<MeterReading> MeterReadings => Set<MeterReading>();
    }
}