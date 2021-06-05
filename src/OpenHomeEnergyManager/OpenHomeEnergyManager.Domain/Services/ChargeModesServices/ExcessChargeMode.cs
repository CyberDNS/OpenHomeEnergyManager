using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public class ExcessChargeMode : IChargeMode
    {
        private ExcessChargeModeData _data = new ExcessChargeModeData();

        private readonly ILogger _logger;
        private readonly ChargePointService _chargePointService;
        private readonly VehicleService _vehicleService;
        private readonly IModuleService _smaHomeManagerService;

        public string UniqueIdentifier => "EXCESS";

        public ExcessChargeMode(ILogger<ExcessChargeMode> logger, ChargePointService chargePointService, VehicleService vehicleService, IModuleServiceRegistry moduleServiceRegistry)
        {
            _logger = logger;
            _chargePointService = chargePointService;
            _vehicleService = vehicleService;
            _smaHomeManagerService = moduleServiceRegistry.FindById(9);
        }

        public async Task LoopAsync(ChargePoint chargePoint, Vehicle vehicle)
        {
            int exportedPower = _smaHomeManagerService.GetCapability<PowerCapability>("EXPORTED_POWER").Value;
            int importedPower = _smaHomeManagerService.GetCapability<PowerCapability>("IMPORTED_POWER").Value;

            //int power = (exportedPower - importedPower) + 2000;

            //exportedPower = 0;
            //importedPower = 0;
            //if (power >= 0) { exportedPower = power; }
            //else { importedPower = Math.Abs(power); }

            _logger.LogDebug("POWER / Ex: {ExportedPower} Im: {ImportedPower} Last Im: {lastImport}", exportedPower, importedPower, _data.LastImportedAt);
            _logger.LogDebug("CHARGE / Started: {Started} Stopped: {Stopped}", _data.StartedChargingAt, _data.StoppedChargingAt);

            ChargePointDataset chargePointData = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value);
            VehicleDataset vehicleData = _vehicleService.GetCurrentData(vehicle.ModuleId);

            bool isCharging = GetIsCharging(chargePoint, vehicle);
            bool isNotCharging = GetIsNotCharging(chargePoint, vehicle);

            if (exportedPower > 0)
            {
                _data.LastImportedAt = null;
            }
            else if ((importedPower > 0) && !_data.LastImportedAt.HasValue)
            {
                _data.LastImportedAt = DateTime.UtcNow;
            }


            if (exportedPower > 6 * 230)
            {
                var minutesAfterLastStop = (DateTime.UtcNow - _data.StoppedChargingAt).TotalMinutes;

                if ((isNotCharging) && (minutesAfterLastStop > 5))
                {
                    _logger.LogInformation("Starting charge / Minutes after last stop: {minutesAfterLastStop}", minutesAfterLastStop);
                    await StartCharging(chargePoint, vehicle);
                    _logger.LogInformation("Started!");
                }
            }
            else if (importedPower > 3 * 230)
            {
                var importingSinceMinutes = (DateTime.UtcNow - _data.LastImportedAt.Value).TotalMinutes;

                if (isCharging && importingSinceMinutes > 10)
                {
                    _logger.LogInformation("Stopping charge / Importing since minutes: {importingSinceMinutes}", importingSinceMinutes);
                    await StopCharging(chargePoint, vehicle);
                    _logger.LogInformation("Stopped!");
                }
            }

            if ((exportedPower > _data.Phases * 230) && isCharging)
            {
                int currentCharge = Convert.ToInt32(Math.Round(chargePointData.CurrentPhase1, 0));
                int deltaCharge = Convert.ToInt32(Math.Round((decimal)exportedPower / (_data.Phases * 230), 0));

                if ((deltaCharge + currentCharge > 19) && (DateTime.UtcNow - _data.PhasesChangeTo1At).TotalMinutes > 5)
                {
                    if (chargePointData.PhaseCount != 3)
                    {
                        _data.Phases = 3;
                        _data.PhasesChangeTo3At = DateTime.UtcNow;
                        _data.LastCurrentChangeAt = DateTime.UtcNow;

                        _logger.LogInformation("Setting phases: {phases}", _data.Phases);
                        await SetPhases(chargePoint, vehicle, _data.Phases);
                    }
                }
                else
                {
                    int newCharge = Math.Min(currentCharge + deltaCharge, 16);

                    if ((currentCharge != newCharge) && (DateTime.UtcNow - _data.LastCurrentChangeAt).TotalSeconds > (deltaCharge * 10))
                    {
                        _data.LastCurrentChangeAt = DateTime.UtcNow;

                        _logger.LogInformation("Increase Current: {old} >>> {new}", currentCharge, newCharge);
                        _chargePointService.SetCurrent(chargePoint.ModuleId.Value, newCharge);
                    }
                }
            }
            else if ((importedPower > _data.Phases * 230) && isCharging)
            {
                int currentCharge = Convert.ToInt32(Math.Round(chargePointData.CurrentPhase1, 0));
                int deltaCharge = Convert.ToInt32(Math.Round((decimal)importedPower / (_data.Phases * 230), 0));
         
                if ((currentCharge - deltaCharge < 6) && (DateTime.UtcNow - _data.PhasesChangeTo3At).TotalMinutes > 5)
                {
                    if (chargePointData.PhaseCount != 1)
                    {
                        _data.Phases = 1;
                        _data.PhasesChangeTo1At = DateTime.UtcNow;
                        _data.LastCurrentChangeAt = DateTime.UtcNow;

                        _logger.LogInformation("Setting phases: {phases}", _data.Phases);
                        await SetPhases(chargePoint, vehicle, _data.Phases);
                    }
                }
                else
                {
                    int newCharge = Math.Max(currentCharge - deltaCharge, 6);

                    if ((currentCharge != newCharge) && (DateTime.UtcNow - _data.LastCurrentChangeAt).TotalSeconds > (deltaCharge * 10))
                    {
                        _data.LastCurrentChangeAt = DateTime.UtcNow;

                        _logger.LogInformation("Decrease Current: {old} >>> {new}", currentCharge, newCharge);
                        _chargePointService.SetCurrent(chargePoint.ModuleId.Value, newCharge);
                    }
                }
            }
        }

        public async Task StartCharging(ChargePoint chargePoint, Vehicle vehicle, int phases = 1)
        {
            _chargePointService.SetCurrent(chargePoint.ModuleId.Value, 6);
            _chargePointService.SetPhaseCount(chargePoint.ModuleId.Value, phases);

            if (vehicle.ChargerMustBeOffOnChanges)
            {
                _vehicleService.SetIsCharging(vehicle.ModuleId, true);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            var result = Policy.HandleResult<bool>(b => b)
                .WaitAndRetry(6, r => TimeSpan.FromSeconds(10))
                .ExecuteAndCapture(() => GetIsCharging(chargePoint, vehicle));

            if (result.Outcome == OutcomeType.Failure) { _logger.LogError("Car did not start charging"); }
        }

        public async Task StopCharging(ChargePoint chargePoint, Vehicle vehicle)
        {
            if (vehicle.ChargerMustBeOffOnChanges)
            {
                _vehicleService.SetIsCharging(vehicle.ModuleId, false);
                await Task.Delay(TimeSpan.FromSeconds(5));

                var result = Policy.HandleResult<bool>(b => b)
                    .WaitAndRetry(6, r => TimeSpan.FromSeconds(10))
                    .ExecuteAndCapture(() => GetIsNotCharging(chargePoint, vehicle));

                if (result.Outcome == OutcomeType.Failure) { _logger.LogError("Car did not start charging"); }
            }

            _chargePointService.SetCurrent(chargePoint.ModuleId.Value, 0);
        }

        public bool GetIsCharging(ChargePoint chargePoint, Vehicle vehicle)
        {
            bool chargePointIsCharging = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsCharging;
            bool vehicleIsCharging = _vehicleService.GetCurrentData(vehicle.ModuleId).IsCharging;

            return chargePointIsCharging && vehicleIsCharging;
        }

        public bool GetIsNotCharging(ChargePoint chargePoint, Vehicle vehicle)
        {
            bool chargePointIsCharging = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsCharging;
            bool vehicleIsCharging = _vehicleService.GetCurrentData(vehicle.ModuleId).IsCharging;

            return !chargePointIsCharging && !vehicleIsCharging;
        }

        public async Task SetPhases(ChargePoint chargePoint, Vehicle vehicle, int phases)
        {
            await StopCharging(chargePoint, vehicle);
            await StartCharging(chargePoint, vehicle, phases);
        }
    }
}
