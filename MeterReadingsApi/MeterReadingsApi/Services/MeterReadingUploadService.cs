using FluentValidation;
using FluentValidation.Results;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Repositories;
using System.Globalization;
using System.IO;

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
            int success = 0;
            int failed = 0;

            foreach (MeterReadingCsvRecord record in records)
            {
                ValidationResult result = validator.Validate(record);
                if (!result.IsValid)
                {
                    failed++;
                    continue;
                }

                int value = int.Parse(record.MeterReadValue);
                DateTime date = DateTime.ParseExact(record.MeterReadingDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                validReadings.Add(new MeterReading
                {
                    AccountId = record.AccountId,
                    MeterReadingDateTime = date,
                    MeterReadValue = value
                });

                success++;
            }

            if (validReadings.Count > 0)
            {
                await repository.AddMeterReadingsAsync(validReadings);
            }

            return new Models.MeterReadingUploadResult(success, failed);
        }
    }
}