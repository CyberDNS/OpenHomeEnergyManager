using AutoMapper;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Dtos
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
