using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OpenHomeEnergyManager.Api.Controllers.V1.Vehicles.Commands;
using OpenHomeEnergyManager.Api.Controllers.V1.Vehicles.Queries;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Vehicles
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddVehicleDto command)
        {
            Vehicle entity = _vehicleRepository.Add(new Vehicle(command.Name));
            await _vehicleRepository.UnitOfWork.SaveEntitiesAsync();

            return CreatedAtAction("Get", entity);
        }

        [HttpPut("{id}/Commands/UpdateInformation")]
        public async Task<IActionResult> UpdateInformation(int id, [FromBody] UpdateVehicleInformationDto command)
        {
            var vehicle = await _vehicleRepository.FindByIdAsync(id);
            vehicle.Name = command.Name;
            vehicle.ModuleId = command.ModuleId;

            await _vehicleRepository.UnitOfWork.SaveEntitiesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Vehicle vehicle = await _vehicleRepository.FindByIdAsync(id);
            _vehicleRepository.Remove(vehicle);
            await _vehicleRepository.UnitOfWork.SaveEntitiesAsync();

            return Accepted();
        }
    }
}
