namespace MeterReadingsApi.Controllers
{
    using MeterReadingsApi.Interfaces;
    using MeterReadingsApi.DataModel;
    using MeterReadingsApi.Repositories;
    using MeterReadingsApi.Models;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/meter-readings")]
    [ApiController]
    public class MeterReadingsController : ControllerBase
    {
        private readonly ICSVService csvService;
        private readonly IConfiguration configuration;
        private readonly IMeterReadingsRepository repository;

        public MeterReadingsController(ICSVService csvService, IConfiguration configuration, IMeterReadingsRepository repository)
        {
            this.csvService = csvService;
            this.configuration = configuration;
            this.repository = repository;
        }

        [Route("")]
        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }

        [Route("~/accounts")]
        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAccounts()
        {
            var accounts = repository.GetAccounts();
            return Ok(accounts);
        }

        [Route("~/accounts/{id}/meter-readings")]
        [HttpGet]
        public ActionResult GetByAccountId(int accountId)
        {
            return Ok();
        }

        [Route("meter-reading-uploads")]
        [HttpPost]
        public async Task<ActionResult> MeterReadingUploads(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }

            IEnumerable<MeterReadingCsvRecord> records;
            try
            {
                using var stream = file.OpenReadStream();
                records = await csvService.ReadMeterReadingsAsync(stream);
            }
            catch
            {
                return BadRequest();
            }

            var validReadings = new List<MeterReading>();
            int success = 0;
            int failed = 0;

            foreach (var record in records)
            {
                if (!repository.AccountExists(record.AccountId))
                {
                    failed++;
                    continue;
                }

                if (!int.TryParse(record.MeterReadValue, out var value) || value < 0 || value > 99999)
                {
                    failed++;
                    continue;
                }

                if (repository.ReadingExists(record.AccountId, record.MeterReadingDateTime))
                {
                    failed++;
                    continue;
                }

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

            if (success == 0)
            {
                return UnprocessableEntity(new { successful = success, failed });
            }

            if (failed > 0)
            {
                return StatusCode(StatusCodes.Status207MultiStatus, new { successful = success, failed });
            }

            return StatusCode(StatusCodes.Status201Created, new { successful = success, failed });
        }
    }
}