using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;
using Polly;
using System;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public class ChargeModeBase
    {
        protected readonly ILogger<ChargeModeBase> _logger;
        protected readonly ChargePointService _chargePointService;
        protected readonly VehicleService _vehicleService;

        public ChargeModeBase(ILogger<ChargeModeBase> logger, ChargePointService chargePointService, VehicleService vehicleService)
        {
            _logger = logger;
            _chargePointService = chargePointService;
            _vehicleService = vehicleService;
        }

        protected bool GetIsCharging(ChargePoint chargePoint, Vehicle vehicle)
        {
            bool chargePointIsCharging = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsCharging;
            bool vehicleIsCharging = _vehicleService.GetCurrentData(vehicle.ModuleId).IsCharging;

            _logger.LogInformation("IsCharging: CP: {chargePointIsCharging} V: {vehicleIsCharging}", chargePointIsCharging, vehicleIsCharging);

            return chargePointIsCharging && vehicleIsCharging;
        }

        protected bool GetIsNotCharging(ChargePoint chargePoint, Vehicle vehicle)
        {
            bool chargePointIsCharging = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsCharging;
            bool vehicleIsCharging = _vehicleService.GetCurrentData(vehicle.ModuleId).IsCharging;

            _logger.LogInformation("IsNotCharging: CP: {chargePointIsCharging} V: {vehicleIsCharging}", chargePointIsCharging, vehicleIsCharging);

            return !chargePointIsCharging && !vehicleIsCharging;
        }

        protected void SetCurrent(ChargePoint chargePoint, int current)
        {
            _chargePointService.SetCurrent(chargePoint.ModuleId.Value, current);
        }

        protected async Task SetPhases(ChargePoint chargePoint, Vehicle vehicle, int phases)
        {
            await StopCharging(chargePoint, vehicle);
            await StartCharging(chargePoint, vehicle, phases);
        }

        protected async Task StartCharging(ChargePoint chargePoint, Vehicle vehicle, int phases = 1)
        {
            _chargePointService.SetCurrent(chargePoint.ModuleId.Value, 7);
            _chargePointService.SetPhaseCount(chargePoint.ModuleId.Value, phases);

            _vehicleService.SetIsCharging(vehicle.ModuleId, true);
            await Task.Delay(TimeSpan.FromSeconds(5));

            var result = Policy.HandleResult<bool>(b => b)
                .WaitAndRetry(6, r => TimeSpan.FromSeconds(10))
                .ExecuteAndCapture(() =>
                {
                    bool isCharging = GetIsCharging(chargePoint, vehicle);
                    if (!isCharging) { _logger.LogWarning("Car did not start charging. Retrying..."); }

                    return !isCharging;
                });

            if (result.Outcome == OutcomeType.Failure) { _logger.LogError("Car did not start charging"); }
            else { _logger.LogInformation("Charge started!"); }

        }

        protected async Task StopCharging(ChargePoint chargePoint, Vehicle vehicle)
        {
            _vehicleService.SetIsCharging(vehicle.ModuleId, false);
            await Task.Delay(TimeSpan.FromSeconds(5));

            var result = Policy.HandleResult<bool>(b => b)
                .WaitAndRetry(6, r => TimeSpan.FromSeconds(10))
                .ExecuteAndCapture(() =>
                {
                    bool isNotCharging = GetIsNotCharging(chargePoint, vehicle);
                    if (!isNotCharging) { _logger.LogWarning("Car did not stop charging. Retrying..."); }

                    return !isNotCharging;
                });


            if (result.Outcome == OutcomeType.Failure) { _logger.LogError("Car did not stop charging"); }
            else { _logger.LogInformation("Charge stopped!"); }

            _chargePointService.SetCurrent(chargePoint.ModuleId.Value, 0);
        }
    }
}