using Microsoft.EntityFrameworkCore;

namespace MeterReadingsApi.DataModel
{
    public class MeterReadingsContext : DbContext
    {
        public MeterReadingsContext(DbContextOptions<MeterReadingsContext> options)
            : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<MeterReading> MeterReadings => Set<MeterReading>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Account primary key
            modelBuilder.Entity<Account>()
                        .HasKey(a => a.AccountId);

            // MeterReading composite key + FK
            modelBuilder.Entity<MeterReading>()
                        .HasKey(m => new { m.AccountId, m.MeterReadingDateTime });

            modelBuilder.Entity<MeterReading>()
                        .HasOne(m => m.Account)
                        .WithMany(a => a.MeterReadings)
                        .HasForeignKey(m => m.AccountId);
        }
    }
}