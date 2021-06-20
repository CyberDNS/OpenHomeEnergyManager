using AutoMapper;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;

namespace OpenHomeEnergyManager.Api.Controllers.V1.ChargePoints.Queries
{
    public class ChargePointProfile : Profile
    {
        public ChargePointProfile()
        {
            CreateMap<ChargePoint, ChargePointDto>();
            CreateMap<ChargePointDto, ChargePoint>();
        }
    }
}
