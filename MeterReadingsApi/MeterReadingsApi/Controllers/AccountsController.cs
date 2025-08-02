using AutoMapper;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Models;
using MeterReadingsApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingsApi.Controllers
{
    [Route("~/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMeterReadingsRepository repository;
        private readonly IMapper mapper;

        public AccountsController(IMeterReadingsRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult Get()
        {
            IEnumerable<Account> accounts = repository.GetAccounts();

            if (!accounts.Any())
            {
                return NoContent();
            }

            IEnumerable<AccountDto> result = mapper.Map<IEnumerable<AccountDto>>(accounts);
            return Ok(result);
        }
    }
}