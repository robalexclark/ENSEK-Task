using MeterReadingsApi.DataModel;
using MeterReadingsApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingsApi.Controllers
{
    [Route("~/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMeterReadingsRepository repository;

        public AccountsController(IMeterReadingsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult Get()
        {
            IEnumerable<Account> accounts = repository.GetAccounts();

            if (!accounts.Any())
            {
                return NoContent();
            }

            return Ok(accounts);
        }
    }
}