using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OpenHomeEnergyManager.Api.Controllers.V1.ModuleServiceDefinitions.Queries;
using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;

namespace OpenHomeEnergyManager.Api.Controllers.V1.ModuleServiceDefinitions
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
