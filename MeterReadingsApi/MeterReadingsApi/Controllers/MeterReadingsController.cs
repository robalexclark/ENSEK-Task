using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingsApi.Controllers
{
    [Route("api/meter-readings")]
    [ApiController] // automatic 400s on model-binding errors
    public class MeterReadingsController : ControllerBase
    {
        private readonly IMeterReadingUploadService uploadService;

        public MeterReadingsController(IMeterReadingUploadService uploadService)
        {
            this.uploadService = uploadService;
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
                return BadRequest("File is null or empty.");

            MeterReadingUploadResult result = await uploadService.UploadAsync(file);

            return result switch
            {
                { Successful: > 0, Failed: 0 } => Created(string.Empty, result),   // 201
                { Successful: > 0, Failed: > 0 } => StatusCode(StatusCodes.Status207MultiStatus, result),
                _ => UnprocessableEntity(result)      // 422
            };
        }
    }
}