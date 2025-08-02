using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Models;
using MeterReadingsApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingsApi.Controllers
{
    [ApiController] // automatic 400s on model-binding errors
    public class MeterReadingsController : ControllerBase
    {
        private readonly IMeterReadingUploadService uploadService;
        private readonly IMeterReadingsRepository repository;
        private readonly IValidator<int> accountIdValidator;
        private readonly IValidator<MeterReadingUploadRequest> fileValidator;
        private readonly IMapper mapper;

        public MeterReadingsController(
            IMeterReadingUploadService uploadService,
            IMeterReadingsRepository repository,
            IValidator<int> accountIdValidator,
            IValidator<MeterReadingUploadRequest> fileValidator,
            IMapper mapper)
        {
            this.uploadService = uploadService;
            this.repository = repository;
            this.accountIdValidator = accountIdValidator;
            this.fileValidator = fileValidator;
            this.mapper = mapper;
        }

        [Route("~/accounts/{accountId}/meter-readings")]
        [HttpGet]
        public ActionResult GetByAccountId(int accountId)
        {
            ValidationResult validation = accountIdValidator.Validate(accountId);
            if (!validation.IsValid)
            {
                return NotFound();
            }

            IEnumerable<MeterReading> readings = repository.GetMeterReadingsByAccountId(accountId);

            if (!readings.Any())
            {
                return NoContent();
            }

            IEnumerable<MeterReadingDto> result = mapper.Map<IEnumerable<MeterReadingDto>>(readings);
            return Ok(result);
        }

        [Route("~/meter-reading-uploads")]
        [HttpPost]
        public async Task<ActionResult> MeterReadingUploads([FromForm] MeterReadingUploadRequest request)
        {
            ValidationResult validation = fileValidator.Validate(request);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            MeterReadingUploadResult result = await uploadService.UploadAsync(request.File!);

            return result switch
            {
                { Successful: > 0, Failed: 0 } => Created(string.Empty, result),   // 201
                { Successful: > 0, Failed: > 0 } => StatusCode(StatusCodes.Status207MultiStatus, result),
                _ => UnprocessableEntity(result)      // 422
            };
        }
    }
}