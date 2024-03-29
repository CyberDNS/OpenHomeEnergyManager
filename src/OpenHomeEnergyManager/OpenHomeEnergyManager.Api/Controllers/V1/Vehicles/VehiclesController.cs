﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OpenHomeEnergyManager.Api.Controllers.V1.Vehicles.Commands;
using OpenHomeEnergyManager.Api.Controllers.V1.Vehicles.Queries;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using OpenHomeEnergyManager.Domain.Services.DataHistorizationServices;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Vehicles
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly VehicleService _vehicleService;
        private readonly DataHistorizationService _dataHistorizationService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public VehiclesController(IVehicleRepository vehicleRepository, VehicleService vehicleService, DataHistorizationService dataHistorizationService, IMapper mapper, IWebHostEnvironment env)
        {
            _vehicleRepository = vehicleRepository;
            _vehicleService = vehicleService;
            _dataHistorizationService = dataHistorizationService;
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

        [HttpGet("{id}/Data/Now")]
        public async Task<IActionResult> GetNowData(int id)
        {
            Vehicle vehicle = await _vehicleRepository.FindByIdAsync(id);

            if (vehicle is null || !vehicle.ModuleId.HasValue || vehicle.ModuleId < 1) { return NotFound(); }
            var current = _mapper.Map<VehicleDatasetDto>(_vehicleService.GetCurrentData(vehicle.ModuleId.Value));

            return Ok(current);
        }

        [HttpGet("{id}/Data/Historization")]
        public IActionResult GetHistorizationData(int id, [FromQuery] int minutes = 10)
{
            var historizationDataset = _dataHistorizationService.GetHistorizationDataset<VehicleDataset>(id, TimeSpan.FromSeconds(20));

            var result = historizationDataset.GetData(TimeSpan.FromMinutes(minutes)).Select(d => _mapper.Map<VehicleDatasetDto>(d)).ToArray();

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
