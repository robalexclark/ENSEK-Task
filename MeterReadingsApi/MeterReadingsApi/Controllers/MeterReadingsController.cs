namespace MeterReadingsApi.Controllers
{
    using MeterReadingsApi.Interfaces;
    using MeterReadingsApi.DataModel;
    using MeterReadingsApi.Repositories;
    using System.Collections.Generic;
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
        public ActionResult MeterReadingUploads()
        {
            return Ok();
        }
    }
}