using AutoMapper;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Vehicles.Queries
{
    public class VehicleDatasetProfile : Profile
    {
        public VehicleDatasetProfile()
        {
            CreateMap<VehicleDataset, VehicleDatasetDto>();
            CreateMap<VehicleDatasetDto, VehicleDataset>();
        }
    }
}
