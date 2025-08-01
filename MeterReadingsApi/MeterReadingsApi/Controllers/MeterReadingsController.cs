namespace MeterReadingsApi.Controllers
{
    using MeterReadingsApi.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/meter-readings")]
    [ApiController]
    public class MeterReadingsController : ControllerBase
    {
        private readonly ICSVService csvService;
        private readonly IConfiguration configuration;

        public MeterReadingsController(ICSVService csvService, IConfiguration configuration)
        {
            this.csvService = csvService;
            this.configuration = configuration;
        }

        [Route("")]
        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }

        [Route("~/accounts/{id}/meter-readings")]
        [HttpGet]
        public ActionResult GetByAccountId(int accountId)
        {
            return Ok();
        }

        [Route("meter-reading-uploads")]
        [HttpPost]
        public ActionResult MeterReadingUploads()
        {
            return Ok();
        }
    }
}