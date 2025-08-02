using FluentValidation;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.Repositories;

namespace MeterReadingsApi.Validators
{
    public class MeterReadingCsvRecordValidator : AbstractValidator<MeterReadingCsvRecord>
    {
        public MeterReadingCsvRecordValidator(IMeterReadingsRepository repository)
        {
            RuleFor(r => r.AccountId)
                .Must(repository.AccountExists)
                .WithMessage("Account does not exist");

            RuleFor(r => r.MeterReadValue)
                .NotEmpty()
                .Matches("^\\d{5}$")
                .WithMessage("MeterReadValue must be a 5-digit integer");

            RuleFor(r => r)
                .Must(r => !repository.ReadingExists(r.AccountId, r.MeterReadingDateTime))
                .WithMessage("Reading already exists for this account and date/time");

            RuleFor(r => r)
                .Must(r => !repository.HasNewerReading(r.AccountId, r.MeterReadingDateTime))
                .WithMessage("Reading is older than existing reading");
        }
    }
}