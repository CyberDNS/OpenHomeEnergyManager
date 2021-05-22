using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OpenHomeEnergyManager.Api.Controllers.V1.Dtos;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1
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
        public async Task<IActionResult> Post([FromBody] ModuleDto module)
        {
            Module entity = _moduleRepository.Add(_mapper.Map<Module>(module));
            await _moduleRepository.UnitOfWork.SaveEntitiesAsync();

            return CreatedAtAction("Get", entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ModuleDto module)
        {
            if (module.Id != id) { throw new ArgumentException("Id in dto and query must be the same"); }

            _moduleRepository.Update(_mapper.Map<Module>(module));
            await _moduleRepository.UnitOfWork.SaveEntitiesAsync();

            return Accepted();
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
