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
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("AccountId is required")
                .Must(v => int.TryParse(v, out _)).WithMessage("AccountId must be an integer")
                .Must(v => repository.AccountExists(int.Parse(v))).WithMessage("Account does not exist");

            RuleFor(r => r.MeterReadingDateTime)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("MeterReadingDateTime is required")
                .Must(v => TryParseDate(v, out _))
                .WithMessage("MeterReadingDateTime must be in format dd/MM/yyyy HH:mm");

            RuleFor(r => r.MeterReadValue)
                .NotEmpty()
                .Matches("^\\d{5}$")
                .WithMessage("MeterReadValue must be a 5-digit integer");

            RuleFor(r => r)
                .Must(r =>
                {
                    if (!int.TryParse(r.AccountId, out int accountId))
                        return true;
                    if (!TryParseDate(r.MeterReadingDateTime, out DateTime dt))
                        return true;
                    return !repository.ReadingExists(accountId, dt);
                })
                .WithMessage("Reading already exists for this account and date/time");

            RuleFor(r => r)
                .Must(r =>
                {
                    if (!int.TryParse(r.AccountId, out int accountId))
                        return true;
                    if (!TryParseDate(r.MeterReadingDateTime, out DateTime dt))
                        return true;
                    return !repository.HasNewerReading(accountId, dt);
                })
                .WithMessage("Reading is older than existing reading");
        }

        private static bool TryParseDate(string value, out DateTime date) =>
            DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }
}