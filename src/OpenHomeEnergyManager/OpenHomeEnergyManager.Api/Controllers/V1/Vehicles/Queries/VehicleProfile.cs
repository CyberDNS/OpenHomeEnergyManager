using AutoMapper;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Vehicles.Queries
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile()
        {
            CreateMap<Vehicle, VehicleDto>();
            CreateMap<VehicleDto, Vehicle>();
        }
    }
}
