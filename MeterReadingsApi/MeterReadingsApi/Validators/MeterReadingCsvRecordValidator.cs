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
                .Must(v => int.TryParse(v, out var value) && value >= 0 && value <= 99999)
                .WithMessage("MeterReadValue must be an integer between 0 and 99999");

            RuleFor(r => r)
                .Must(r => !repository.ReadingExists(r.AccountId, r.MeterReadingDateTime))
                .WithMessage("Reading already exists for this account and date");
        }
    }
}
