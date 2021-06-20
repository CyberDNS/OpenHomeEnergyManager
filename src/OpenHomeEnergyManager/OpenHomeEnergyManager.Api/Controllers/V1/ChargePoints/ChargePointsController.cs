using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Api.Controllers.V1.ChargePoints.Commands;
using OpenHomeEnergyManager.Api.Controllers.V1.ChargePoints.Queries;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargeModesServices;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;


namespace OpenHomeEnergyManager.Api.Controllers.V1.ChargePoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargePointsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IChargePointRepository _chargePointRepository;
        private readonly ChargePointService _chargePointService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ChargePointsController(ILogger<ChargePointsController> logger, IChargePointRepository chargePointRepository, ChargePointService chargePointService, IMapper mapper, IWebHostEnvironment env)
        {
            _logger = logger;
            _chargePointRepository = chargePointRepository;
            _chargePointService = chargePointService;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _chargePointRepository.GetAll().Select(c => _mapper.Map<ChargePointDto>(c)).ToArray();
            foreach (var chargePoint in result) { chargePoint.Image = GetImage(chargePoint.Id); }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = _mapper.Map<ChargePointDto>(await _chargePointRepository.FindByIdAsync(id));
            result.Image = GetImage(result.Id);

            return Ok(result);
        }

        private string GetImage(int id)
        {
            Random random = new Random();

            string filename = $"chargepoint_{id}";
            var folderPath = Path.Combine(_env.ContentRootPath, "data", "images");

            var filenameWithExtension = Directory.GetFiles(folderPath, $"{filename}.*").SingleOrDefault();
            if (filenameWithExtension is null) { return null; }

            return $"{Path.GetFileName(filenameWithExtension)}?{random.Next(0, 100000)}";
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddChargePointDto command)
        {
            ChargePoint entity = _chargePointRepository.Add(new ChargePoint(command.Name));
            await _chargePointRepository.UnitOfWork.SaveEntitiesAsync();

            return CreatedAtAction("Get", entity);
        }

        [HttpGet("{id}/Data/Now")]
        public async Task<IActionResult> GetNowData(int id)
        {
            ChargePoint chargePoint = await _chargePointRepository.FindByIdAsync(id);

            if (chargePoint is null || !chargePoint.ModuleId.HasValue || chargePoint.ModuleId < 1) { return NotFound(); }
            var current = _mapper.Map<ChargePointDatasetDto>(_chargePointService.GetCurrentData(chargePoint.ModuleId.Value));

            return Ok(current);
        }

        [HttpPut("{id}/Commands/SetCurrent")]
        public async Task<IActionResult> SetCurrent(int id, [FromBody] SetCurrentDto command)
        {
            ChargePoint chargePoint = await _chargePointRepository.FindByIdAsync(id);

            if (!chargePoint.ModuleId.HasValue) { return NotFound(); }
            _chargePointService.SetCurrent(chargePoint.ModuleId.Value, command.Current);

            return NoContent();
        }

        [HttpPut("{id}/Commands/SelectChargeMode")]
        public async Task<IActionResult> SelectChargeMode(int id, [FromBody] SelectChargeModeDto command)
        {
            ChargePoint chargePoint = await _chargePointRepository.FindByIdAsync(id);
            chargePoint.CurrentChargeMode = Enum.Parse<ChargeModes>(command.ChargeMode.ToString());
            await _chargePointRepository.UnitOfWork.SaveEntitiesAsync();

            return NoContent();
        }

        [HttpPut("{id}/Commands/UpdateInformation")]
        public async Task<IActionResult> UpdateInformation(int id, [FromBody] UpdateChargePointInformationDto command)
        {
            var chargePoint = await _chargePointRepository.FindByIdAsync(id);
            chargePoint.Name = command.Name;
            chargePoint.ModuleId = command.ModuleId;

            await _chargePointRepository.UnitOfWork.SaveEntitiesAsync();

            return NoContent();
        }

        [HttpPut("{id}/Commands/AttributeVehicle")]
        public async Task<IActionResult> AttributeVehicle(int id, [FromBody] AttributeVehicleDto command)
        {
            var chargePoint = await _chargePointRepository.FindByIdAsync(id);
            chargePoint.VehicleId = command.VehicleId;
            await _chargePointRepository.UnitOfWork.SaveEntitiesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ChargePoint chargePoint = await _chargePointRepository.FindByIdAsync(id);

            _chargePointRepository.Remove(chargePoint);
            await _chargePointRepository.UnitOfWork.SaveEntitiesAsync();

            return Accepted();
        }
    }
}
