﻿using CsvHelper;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.Interfaces;
using System.Globalization;
using System.Text;
using System.IO;

namespace MeterReadingsApi.Services
{
    public class CsvService : ICSVService
    {
        public async Task<IEnumerable<MeterReadingCsvRecord>> ReadMeterReadingsAsync(Stream stream)
        {
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<MeterReadingCsvMap>();
            List<MeterReadingCsvRecord> records = new List<MeterReadingCsvRecord>();

            await foreach (MeterReadingCsvRecord record in csv.GetRecordsAsync<MeterReadingCsvRecord>())
            {
                records.Add(record);
            }

            return records;
        }
    }
}