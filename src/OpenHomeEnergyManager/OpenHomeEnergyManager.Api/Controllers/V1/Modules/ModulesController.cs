using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OpenHomeEnergyManager.Api.Controllers.V1.Modules.Commands;
using OpenHomeEnergyManager.Api.Controllers.V1.Modules.Queries;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Modules
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IMapper _mapper;
        private readonly IHostedService _hostedService;

        public ModulesController(IModuleRepository moduleRepository, IMapper mapper, IHostedService hostedService)
        {
            _moduleRepository = moduleRepository;
            _mapper = mapper;
            _hostedService = hostedService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _moduleRepository.GetAll().Select(c => _mapper.Map<Module>(c)).ToArray();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddModuleDto command)
        {
            Module entity = _moduleRepository.Add(new Module(command.Name));
            await _moduleRepository.UnitOfWork.SaveEntitiesAsync();

            return CreatedAtAction("Get", entity);
        }

        [HttpPut("{id}/Commands/UpdateInformation")]
        public async Task<IActionResult> UpdateInformation(int id, [FromBody] UpdateModuleInformationDto command)
        {
            var module = await _moduleRepository.FindByIdAsync(id);
            module.Name = command.Name;
            module.ModuleServiceDefinitionKey = command.ModuleServiceDefinitionKey;
            module.Settings = command.Settings;
   
            await _moduleRepository.UnitOfWork.SaveEntitiesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Module module = await _moduleRepository.FindByIdAsync(id);

            _moduleRepository.Remove(module);
            await _moduleRepository.UnitOfWork.SaveEntitiesAsync();

            return Accepted();
        }
    }
}
