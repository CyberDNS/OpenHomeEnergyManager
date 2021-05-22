using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OpenHomeEnergyManager.Api.Controllers.V1.Dtos;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OpenHomeEnergyManager.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargePointsController : ControllerBase
    {
        private readonly IChargePointRepository _chargePointRepository;
        private readonly ChargePointService _chargePointService;
        private readonly IMapper _mapper;

        public ChargePointsController(IChargePointRepository chargePointRepository, ChargePointService chargePointService, IMapper mapper)
        {
            _chargePointRepository = chargePointRepository;
            _chargePointService = chargePointService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
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


        [HttpGet("{id}/Data/Current")]
        public async Task<IActionResult> GetCurrentData(int id)
        {
            ChargePoint chargePoint = await _chargePointRepository.FindByIdAsync(id);
            var current = _chargePointService.GetCurrentData(chargePoint.ModuleId);

            return Ok(current);
        }

    }
}
