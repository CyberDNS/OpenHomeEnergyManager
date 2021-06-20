using AutoMapper;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Modules.Queries
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<Module, ModuleDto>();
            CreateMap<ModuleDto, Module>();
        }
    }
}
