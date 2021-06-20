using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;

        public VehiclesController(IVehicleRepository vehicleRepository, VehicleService vehicleService, IMapper mapper, IWebHostEnvironment env)
        {
            _vehicleRepository = vehicleRepository;
            _vehicleService = vehicleService;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _vehicleRepository.GetAll().Select(c => _mapper.Map<VehicleDto>(c)).ToArray();
            foreach (var vehicle in result) { vehicle.Image = GetImage(vehicle.Id); }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = _mapper.Map<VehicleDto>(await _vehicleRepository.FindByIdAsync(id));
            result.Image = GetImage(result.Id);

            return Ok(result);
        }

        private string GetImage(int id)
        {
            Random random = new Random();
            string filename = $"vehicle_{id}";
            var folderPath = Path.Combine(_env.ContentRootPath, "data", "images");

            var filenameWithExtension = Directory.GetFiles(folderPath, $"{filename}.*").SingleOrDefault();
            if (filenameWithExtension is null) { return null; }

            return $"{Path.GetFileName(filenameWithExtension)}?{random.Next(0, 100000)}";
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
