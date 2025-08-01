namespace MeterReadingsApi.Controllers
{
    using MeterReadingsApi.Interfaces;
    using MeterReadingsApi.DataModel;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/meter-readings")]
    [ApiController]
    public class MeterReadingsController : ControllerBase
    {
        private readonly ICSVService csvService;
        private readonly IConfiguration configuration;
        private readonly MeterReadingsContext dbContext;

        public MeterReadingsController(ICSVService csvService, IConfiguration configuration, MeterReadingsContext dbContext)
        {
            this.csvService = csvService;
            this.configuration = configuration;
            this.dbContext = dbContext;
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
            var accounts = dbContext.Accounts.AsNoTracking().ToList();
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