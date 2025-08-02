using FluentValidation;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.Repositories;
using System.Globalization;

namespace MeterReadingsApi.Validators
{
    public class MeterReadingCsvRecordValidator : AbstractValidator<MeterReadingCsvRecord>
    {
        public MeterReadingCsvRecordValidator(IMeterReadingsRepository repository)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.AccountId)
                .Must(repository.AccountExists)
                .WithMessage("Account does not exist");

            RuleFor(r => r.MeterReadingDateTime)
                .Must(v => TryParseDate(v, out _))
                .WithMessage("MeterReadingDateTime must be in format dd/MM/yyyy HH:mm");

            RuleFor(r => r.MeterReadValue)
                .NotEmpty()
                .Matches("^\\d{5}$")
                .WithMessage("MeterReadValue must be a 5-digit integer");

            RuleFor(r => r)
                .Must(r =>
                {
                    if (!TryParseDate(r.MeterReadingDateTime, out DateTime dt))
                        return true;
                    return !repository.ReadingExists(r.AccountId, dt);
                })
                .WithMessage("Reading already exists for this account and date/time");

            RuleFor(r => r)
                .Must(r =>
                {
                    if (!TryParseDate(r.MeterReadingDateTime, out DateTime dt))
                        return true;
                    return !repository.HasNewerReading(r.AccountId, dt);
                })
                .WithMessage("Reading is older than existing reading");
        }

        private static bool TryParseDate(string value, out DateTime date) =>
            DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }
}