using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenHomeEnergyManager.Api.Controllers.V1.Dtos;
using OpenHomeEnergyManager.Api.Controllers.V1.Dtos.Commands;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly VehicleService _vehicleService;
        private readonly IMapper _mapper;

        public VehiclesController(IVehicleRepository vehicleRepository, VehicleService vehicleService, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _vehicleService = vehicleService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _vehicleRepository.GetAll().Select(c => _mapper.Map<VehicleDto>(c)).ToArray();

            return Ok(result);
        }

        [HttpPut("ChangeModule/")]
        public async Task<IActionResult> ChangeModule(ChangeModuleDto dto)
        {
            var vehicle = await _vehicleRepository.FindByIdAsync(dto.Id);
            vehicle.ModuleId = dto.ModuleId;
            await _vehicleRepository.UnitOfWork.SaveEntitiesAsync();

            return NoContent();
        }
    }
}
