using FluentValidation;
using MeterReadingsApi.Models;

namespace MeterReadingsApi.Validators
{
    public class MeterReadingUploadRequestValidator : AbstractValidator<MeterReadingUploadRequest>
    {
        private static readonly string[] ExpectedHeaders = new[] { "AccountId", "MeterReadingDateTime", "MeterReadValue" };

        public MeterReadingUploadRequestValidator()
        {
            RuleFor(x => x.File)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("File is required")
                .Must(f => f!.Length > 0).WithMessage("File is empty")
                .Must(HaveValidHeaders).WithMessage("Invalid or missing headers. Expected: AccountId,MeterReadingDateTime,MeterReadValue")
                .Must(HasNoBlankLines).WithMessage("File contains blank rows");
        }

        private static bool HaveValidHeaders(IFormFile? file)
        {
            using StreamReader reader = new StreamReader(file!.OpenReadStream());
            string? headerLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                return false;
            }

            string[] headers = headerLine.Split(',');
            if (headers.Length != ExpectedHeaders.Length)
            {
                return false;
            }

            return headers.Select(h => h.Trim())
                          .SequenceEqual(ExpectedHeaders, StringComparer.OrdinalIgnoreCase);
        }

        private static bool HasNoBlankLines(IFormFile? file)
        {
            using StreamReader reader = new StreamReader(file!.OpenReadStream());
            // Skip header
            reader.ReadLine();

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    return false;
                }
            }

            return true;
        }
    }
}