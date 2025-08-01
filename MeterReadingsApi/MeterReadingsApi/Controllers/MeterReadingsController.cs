namespace MeterReadingsApi.Controllers
{
    using MeterReadingsApi.Interfaces;
    using MeterReadingsApi.DataModel;
    using MeterReadingsApi.Repositories;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/meter-readings")]
    [ApiController]
    public class MeterReadingsController : ControllerBase
    {
        private readonly IMeterReadingUploadService uploadService;
        private readonly IConfiguration configuration;
        private readonly IMeterReadingsRepository repository;

        public MeterReadingsController(IMeterReadingUploadService uploadService, IConfiguration configuration, IMeterReadingsRepository repository)
        {
            this.uploadService = uploadService;
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

            try
            {
                var (success, failed) = await uploadService.UploadAsync(file);

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
            catch
            {
                return BadRequest();
            }
        }
    }
}