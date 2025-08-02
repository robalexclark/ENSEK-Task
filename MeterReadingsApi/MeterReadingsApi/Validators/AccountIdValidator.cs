using FluentValidation;
using MeterReadingsApi.Repositories;

namespace MeterReadingsApi.Validators
{
    public class AccountIdValidator : AbstractValidator<int>
    {
        public AccountIdValidator(IMeterReadingsRepository repository)
        {
            RuleFor(id => id)
                .Must(repository.AccountExists)
                .WithMessage("Account does not exist");
        }
    }
}