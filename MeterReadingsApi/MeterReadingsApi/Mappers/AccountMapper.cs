using AutoMapper;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Shared;

namespace MeterReadingsApi.Mappers
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<Account, AccountDto>();
        }
    }
}