﻿using FluentValidation;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Repositories;

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
            using var stream = file.OpenReadStream();
            var records = await csvService.ReadMeterReadingsAsync(stream);

            var validReadings = new List<MeterReading>();
            int success = 0;
            int failed = 0;

            foreach (var record in records)
            {
                var result = validator.Validate(record);
                if (!result.IsValid)
                {
                    failed++;
                    continue;
                }

                int value = int.Parse(record.MeterReadValue);

                validReadings.Add(new MeterReading
                {
                    AccountId = record.AccountId,
                    MeterReadingDateTime = record.MeterReadingDateTime,
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