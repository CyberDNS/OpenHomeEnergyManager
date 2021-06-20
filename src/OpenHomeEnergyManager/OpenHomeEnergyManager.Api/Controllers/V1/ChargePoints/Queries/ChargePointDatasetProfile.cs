using AutoMapper;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;

namespace OpenHomeEnergyManager.Api.Controllers.V1.ChargePoints.Queries
{
    public class ChargePointDatasetProfile : Profile
    {
        public ChargePointDatasetProfile()
        {
            CreateMap<ChargePointDataset, ChargePointDatasetDto>();
            CreateMap<ChargePointDatasetDto, ChargePointDataset>();
        }
    }
}
