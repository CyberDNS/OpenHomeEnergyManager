using AutoMapper;
using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;

namespace OpenHomeEnergyManager.Api.Controllers.V1.ModuleServiceDefinitions.Queries
{
    public class ModuleServiceDefinitionProfile : Profile
    {
        public ModuleServiceDefinitionProfile()
        {
            CreateMap<IModuleServiceDefinition, ModuleServiceDefinitionDto>();
            CreateMap<ModuleServiceDefinitionDto, IModuleServiceDefinition>();
        }
    }
}
