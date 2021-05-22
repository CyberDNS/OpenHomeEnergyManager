using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenHomeEnergyManager.Api.Controllers.V1.Dtos;
using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleServiceDefinitionsController : ControllerBase
    {
        private readonly IModuleServiceDefinitionRepository _repository;
        private readonly IMapper _mapper;

        public ModuleServiceDefinitionsController(IModuleServiceDefinitionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _repository.GetAll().Select(c => _mapper.Map<ModuleServiceDefinitionDto>(c)).ToArray();

            return Ok(result);
        }
    }
}
