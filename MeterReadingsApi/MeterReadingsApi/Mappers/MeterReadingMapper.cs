using AutoMapper;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Shared;

namespace MeterReadingsApi.Mappers
{
    public class MeterReadingMapper : Profile
    {
        public MeterReadingMapper()
        {
            CreateMap<MeterReading, MeterReadingDto>();
        }
    }
}