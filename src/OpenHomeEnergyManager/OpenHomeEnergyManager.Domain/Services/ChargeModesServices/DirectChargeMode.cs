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
    public class DirectChargeMode : ChargeModeBase, IChargeMode
    {
        public ChargeModes ChargeMode => ChargeModes.Direct;

        public DirectChargeMode(ILogger<ChargeModeBase> logger, ChargePointService chargePointService, VehicleService vehicleService)
            : base(logger, chargePointService, vehicleService)
        {
        }

        public async Task LoopAsync(ChargePoint chargePoint, Vehicle vehicle)
        {
            ChargePointDataset chargePointData = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value);
            VehicleDataset vehicleData = _vehicleService.GetCurrentData(vehicle.ModuleId.Value);

            bool isCharging = GetIsCharging(chargePoint, vehicle);
            bool isNotCharging = GetIsNotCharging(chargePoint, vehicle);

            if (isCharging == isNotCharging) { _logger.LogWarning("Inconsistent charging state"); }

            bool chargePointIsCharging = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsCharging;

            if (!chargePointIsCharging)
            {
                _logger.LogInformation("Starting charge...");
                await StartCharging(chargePoint, vehicle, phases: 3);
                _logger.LogInformation("Started!");
            }

            if (chargePointData.PhaseCount != 3)
            {
                await SetPhases(chargePoint, vehicle, phases: 3);
            }

            if (Math.Round(chargePointData.CurrentPhase1, 0) < 16)
            {
                SetCurrent(chargePoint, current: 16);
            }
        }
    }
}
