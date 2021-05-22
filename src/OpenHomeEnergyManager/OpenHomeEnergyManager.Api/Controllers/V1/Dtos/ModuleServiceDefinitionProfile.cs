using AutoMapper;
using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Dtos
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
