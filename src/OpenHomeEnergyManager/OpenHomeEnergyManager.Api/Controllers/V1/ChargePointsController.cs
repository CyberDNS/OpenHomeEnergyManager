using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Api.Controllers.V1.Dtos;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace OpenHomeEnergyManager.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargePointsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IChargePointRepository _chargePointRepository;
        private readonly ChargePointService _chargePointService;
        private readonly IMapper _mapper;

        public ChargePointsController(ILogger<ChargePointsController> logger, IChargePointRepository chargePointRepository, ChargePointService chargePointService, IMapper mapper)
        {
            _logger = logger;
            _chargePointRepository = chargePointRepository;
            _chargePointService = chargePointService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogError("Tedt");

            var result = _chargePointRepository.GetAll().Select(c => _mapper.Map<ChargePointDto>(c)).ToArray();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChargePointDto chargePoint)
        {
            ChargePoint entity = _chargePointRepository.Add(_mapper.Map<ChargePoint>(chargePoint));
            await _chargePointRepository.UnitOfWork.SaveEntitiesAsync();

            return CreatedAtAction("Get", entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ChargePointDto chargePoint)
        {
            if (chargePoint.Id != id) { throw new ArgumentException("Id in dto and query must be the same"); }

            _chargePointRepository.Update(_mapper.Map<ChargePoint>(chargePoint));
            await _chargePointRepository.UnitOfWork.SaveEntitiesAsync();

            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ChargePoint chargePoint = await _chargePointRepository.FindByIdAsync(id);

            _chargePointRepository.Remove(chargePoint);
            await _chargePointRepository.UnitOfWork.SaveEntitiesAsync();

            return Accepted();
        }


        [HttpGet("{id}/Data/Now")]
        public async Task<IActionResult> GetNowData(int id)
        {
            ChargePoint chargePoint = await _chargePointRepository.FindByIdAsync(id);

            if (!chargePoint.ModuleId.HasValue || chargePoint.ModuleId < 1) { return NotFound(); }
            var current = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value);

            return Ok(current);
        }

        [HttpPut("{id}/Actions/SetCurrent")]
        public async Task<IActionResult> SetCurrent(int id, [FromBody, Range(0, 32)] int current)
        {
            ChargePoint chargePoint = await _chargePointRepository.FindByIdAsync(id);

            if (!chargePoint.ModuleId.HasValue) { return NotFound(); }
            _chargePointService.SetCurrent(chargePoint.ModuleId.Value, current);

            return NoContent();
        }

        [HttpPut("{id}/Actions/SetChargeMode")]
        public async Task<IActionResult> SetChargeMode(int id, [FromBody] string chargeMode)
        {
            ChargePoint chargePoint = await _chargePointRepository.FindByIdAsync(id);
            chargePoint.CurrentChargeMode = chargeMode;
            await _chargePointRepository.UnitOfWork.SaveEntitiesAsync();

            return NoContent();
        }

    }
}
