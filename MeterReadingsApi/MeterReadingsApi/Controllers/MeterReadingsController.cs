namespace MeterReadingsApi.Controllers
{
    using MeterReadingsApi.Interfaces;
    using MeterReadingsApi.Repositories;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/meter-readings")]
    [ApiController]                       // automatic 400s on model-binding errors
    public class MeterReadingsController : ControllerBase
    {
        private readonly IMeterReadingUploadService uploadService;
        private readonly IMeterReadingsRepository repository;

        public MeterReadingsController(IMeterReadingUploadService uploadService, IMeterReadingsRepository repository)
        {
            this.uploadService = uploadService;
            this.repository = repository;
        }

        [Route("~/accounts/{accountId}/meter-readings")]
        [HttpGet]
        public async Task<ActionResult> GetByAccountId(int accountId)
        {
            if (!repository.AccountExists(accountId))
                return NotFound();

            var readings = await repository.GetReadingsByAccountAsync(accountId);
            return Ok(readings);
        }

        [Route("meter-reading-uploads")]
        [HttpPost]
        public async Task<ActionResult> MeterReadingUploads([FromForm] IFormFile? file)
        {
            if (file.Length == 0)
                return BadRequest("File is empty.");

            var result = await uploadService.UploadAsync(file);

            return result switch
            {
                { Successful: > 0, Failed: 0 } => Created(string.Empty, result),   // 201
                { Successful: > 0, Failed: > 0 } => StatusCode(StatusCodes.Status207MultiStatus, result),
                _ => UnprocessableEntity(result)      // 422
            };
        }
    }
}