using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public class PlannedTargetChargeMode : TargetChargeModeBase
    {
        private DirectChargeModeData _data = new DirectChargeModeData();

        private DateTime _plannedTargetDateTimeUtc = new DateTime(2021, 6, 26, 14, 12, 0);
        private double _reserveFactor = 1.2;
        private double _endModePercentage = 5;

        private int _maxPower = 3 * 230 * 16; 

        private int _vehicleCapacity = 76400;


        public override ChargeModes ChargeMode => ChargeModes.PlannedTarget;

        public PlannedTargetChargeMode(ILogger<ChargeModeBase> logger, ChargePointService chargePointService, VehicleService vehicleService)
            : base(logger, chargePointService, vehicleService)
        {
            _targetSoc = 77;
        }


        public override async Task LoopAsync(ChargePoint chargePoint, Vehicle vehicle)
        {
            ChargePointDataset chargePointData = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value);
            VehicleDataset vehicleData = _vehicleService.GetCurrentData(vehicle.ModuleId.Value);

            bool isCharging = GetIsCharging(chargePoint, vehicle);
            bool isNotCharging = GetIsNotCharging(chargePoint, vehicle);

            if (isCharging == isNotCharging) { _logger.LogWarning("Inconsistent charging state"); }

            bool ruleResult = EvaluateRule(chargePoint, vehicle);

            if (isNotCharging && ruleResult)
            {
                _logger.LogInformation("Starting charge...");
                await StartCharging(chargePoint, vehicle, phases: 3);
                _data.StartedChargingAt = DateTime.UtcNow;
                _logger.LogInformation("Started!");
            }

            if (isCharging)
            {
                double socToCharge = (double)(_targetSoc - vehicleData.StateOfCharge);
                bool endPhase = socToCharge <= _endModePercentage;

                if (DateTime.UtcNow - _data.StartedChargingAt > TimeSpan.FromMinutes(1))
                {
                    if (chargePointData.PhaseCount != 3)
                    {
                        await SetPhases(chargePoint, vehicle, phases: 3);
                    }
                }

                if (!endPhase)
                {
                    if (Math.Round(chargePointData.CurrentPhase1, 0) < 16)
                    {
                        SetCurrent(chargePoint, current: 16);
                    }
                }
                else
                {
                    double capacityToCharge = _vehicleCapacity * socToCharge * 0.01;
                    TimeSpan restTimeToTarget = _plannedTargetDateTimeUtc - DateTime.UtcNow;

                    double powerNeeded = capacityToCharge / restTimeToTarget.TotalHours;
                    int currentToSet = Math.Max(16, Math.Min(6, Convert.ToInt32(Math.Round(powerNeeded / 230 / 3, 0))));
                    int currentlySetCurrent = Convert.ToInt32(Math.Round(chargePointData.CurrentPhase1, 0));

                    if (currentlySetCurrent != currentToSet)
                    {
                        SetCurrent(chargePoint, current: currentToSet);
                    }
                }
            }

            if (isCharging && !ruleResult)
            {
                _logger.LogInformation("Stopping charge...");
                await StopCharging(chargePoint, vehicle);
                _logger.LogInformation("Stopped!");
            }
        }

        public override bool EvaluateRule(ChargePoint chargePoint, Vehicle vehicle)
        {
            if (vehicle?.ModuleId is null) { return false; }

            var vehicleData = _vehicleService.GetCurrentData(vehicle.ModuleId.Value);
            if ((vehicleData.StateOfCharge > 0) && (vehicleData.StateOfCharge < _targetSoc))
            {
                double socToCharge = (double)(_targetSoc - vehicleData.StateOfCharge);
                double capacityToCharge = _vehicleCapacity * socToCharge * 0.01;
                TimeSpan durationToChargeCapacity = TimeSpan.FromHours(capacityToCharge / _maxPower);
                TimeSpan durationWithReserve = durationToChargeCapacity * _reserveFactor;

                TimeSpan restTimeToTarget = _plannedTargetDateTimeUtc - DateTime.UtcNow;

                if ((durationWithReserve >= restTimeToTarget) && (restTimeToTarget > TimeSpan.FromHours(-1)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
