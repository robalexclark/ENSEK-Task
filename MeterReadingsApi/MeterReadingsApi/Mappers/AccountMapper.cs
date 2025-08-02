using AutoMapper;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Models;

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