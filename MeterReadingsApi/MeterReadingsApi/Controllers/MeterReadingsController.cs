namespace MeterReadingsApi.Controllers
{
    using MeterReadingsApi.DataModel;
    using MeterReadingsApi.Interfaces;
    using MeterReadingsApi.Repositories;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

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

        [Route("~/accounts/{id}/meter-readings")]
        [HttpGet]
        public ActionResult GetByAccountId(int accountId)
        {
            return Ok();
        }

        [Route("meter-reading-uploads")]
        [HttpPost]
        public async Task<ActionResult> MeterReadingUploads([FromForm] IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }

            try
            {
                var result = await uploadService.UploadAsync(file);

                if (result.Successful == 0)
                {
                    return UnprocessableEntity(new { successful = result.Successful, failed = result.Failed });
                }

                if (result.Failed > 0)
                {
                    return StatusCode(StatusCodes.Status207MultiStatus, new { successful = result.Successful, failed = result.Failed });
                }

                return StatusCode(StatusCodes.Status201Created);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}