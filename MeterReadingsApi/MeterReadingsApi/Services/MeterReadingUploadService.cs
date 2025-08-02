using FluentValidation;
using FluentValidation.Results;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Models;
using MeterReadingsApi.Repositories;
using System.Globalization;

namespace MeterReadingsApi.Services
{
    public class MeterReadingUploadService : IMeterReadingUploadService
    {
        private readonly ICSVService csvService;
        private readonly IMeterReadingsRepository repository;
        private readonly IValidator<MeterReadingCsvRecord> validator;

        public MeterReadingUploadService(ICSVService csvService, IMeterReadingsRepository repository, IValidator<MeterReadingCsvRecord> validator)
        {
            this.csvService = csvService;
            this.repository = repository;
            this.validator = validator;
        }

        public async Task<Models.MeterReadingUploadResult> UploadAsync(IFormFile file)
        {
            using Stream stream = file.OpenReadStream();
            IEnumerable<MeterReadingCsvRecord> records = await csvService.ReadMeterReadingsAsync(stream);

            List<MeterReading> validReadings = new List<MeterReading>();
            List<MeterReadingUploadFailure> failureDetails = new List<MeterReadingUploadFailure>();
            int success = 0;
            int failed = 0;
            int rowNumber = 2; // account for header row

            foreach (MeterReadingCsvRecord record in records)
            {
                ValidationResult result = validator.Validate(record);
                if (!result.IsValid)
                {
                    failed++;
                    string reason = string.Join("; ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                    int? failureAccountId = int.TryParse(record.AccountId, out int parsedAccountId) ? parsedAccountId : null;
                    failureDetails.Add(new MeterReadingUploadFailure(rowNumber, failureAccountId, reason));
                    rowNumber++;
                    continue;
                }

                int accountId = int.Parse(record.AccountId);
                int value = int.Parse(record.MeterReadValue);
                DateTime date = DateTime.ParseExact(record.MeterReadingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                validReadings.Add(new MeterReading
                {
                    AccountId = accountId,
                    MeterReadingDateTime = date,
                    MeterReadValue = value
                });

                success++;
                rowNumber++;
            }

            if (validReadings.Count > 0)
            {
                await repository.AddMeterReadingsAsync(validReadings);
            }

            return new MeterReadingUploadResult(success, failed, failureDetails);
        }
    }
}