using AutoMapper;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Models;

namespace MeterReadingsApi.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountDto>();
        }
    }
}

