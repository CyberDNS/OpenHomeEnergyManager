using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public class StopChargeMode : ChargeModeBase, IChargeMode
    {
        public StopChargeMode(ILogger<ChargeModeBase> logger, ChargePointService chargePointService, VehicleService vehicleService) 
            : base(logger, chargePointService, vehicleService)
        {
        }

        public ChargeModes ChargeMode => ChargeModes.Stop;



        public async Task LoopAsync(ChargePoint chargePoint, Vehicle vehicle)
        {
            ChargePointDataset chargePointData = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value);
            VehicleDataset vehicleData = _vehicleService.GetCurrentData(vehicle.ModuleId.Value);

            bool isCharging = GetIsCharging(chargePoint, vehicle);
            bool isNotCharging = GetIsNotCharging(chargePoint, vehicle);

            if (isCharging == isNotCharging) { _logger.LogWarning("Inconsistent charging state"); }

            if (isCharging)
            {
                _logger.LogInformation("Stopping charge...");
                await StopCharging(chargePoint, vehicle);
                _logger.LogInformation("Stopped!");
            }
        }

      
    }
}
