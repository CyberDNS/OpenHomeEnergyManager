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
    public class ExcessChargeMode : ChargeModeBase, IChargeMode
    {
        private ExcessChargeModeData _data = new ExcessChargeModeData();

        private readonly IModuleService _smaHomeManagerService;

        public ChargeModes ChargeMode => ChargeModes.Excess;

        public ExcessChargeMode(ILogger<ExcessChargeMode> logger, ChargePointService chargePointService, VehicleService vehicleService, IModuleServiceRegistry moduleServiceRegistry)
            : base(logger, chargePointService, vehicleService)
        {
            _smaHomeManagerService = moduleServiceRegistry.FindById(9);
        }

        public async Task LoopAsync(ChargePoint chargePoint, Vehicle vehicle)
        {
            int exportedPower = _smaHomeManagerService.GetCapability<PowerCapability>("EXPORTED_POWER").Value;
            int importedPower = _smaHomeManagerService.GetCapability<PowerCapability>("IMPORTED_POWER").Value;

            _logger.LogDebug("POWER / Ex: {ExportedPower} Im: {ImportedPower} Last Im: {lastImport}", exportedPower, importedPower, _data.LastImportedAt);
            _logger.LogDebug("CHARGE / Started: {Started} Stopped: {Stopped}", _data.StartedChargingAt, _data.StoppedChargingAt);

            ChargePointDataset chargePointData = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value);
            VehicleDataset vehicleData = _vehicleService.GetCurrentData(vehicle.ModuleId.Value);

            if (vehicleData.IsChargedToChargeLimit) 
            {
                _logger.LogInformation("Vehicle {Name} is charge to its limit of {Limit}", vehicle.Name, vehicleData.ChargeLimit);
                return;
            }

            bool isCharging = GetIsCharging(chargePoint, vehicle);
            bool isNotCharging = GetIsNotCharging(chargePoint, vehicle);

            bool chargePointIsCharging = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsCharging;

            if (isCharging == isNotCharging) { _logger.LogWarning("Inconsistent charging state"); }

            if (exportedPower > 0)
            {
                _data.LastImportedAt = null;
            }
            else if ((importedPower > 0) && !_data.LastImportedAt.HasValue)
            {
                _data.LastImportedAt = DateTime.UtcNow;
            }

            if (isNotCharging)
            {
                _logger.LogInformation("Missing export power to start charging: {MissingPower}W", (6 * 230) - exportedPower);
            }


            if (exportedPower > 6 * 230)
            {
                var minutesAfterLastStop = (DateTime.UtcNow - _data.StoppedChargingAt).TotalMinutes;

                if (isNotCharging)
                {
                    _logger.LogInformation("Start charging in {Minutes}", Math.Round(5 - minutesAfterLastStop, 2));

                    if ((minutesAfterLastStop > 5))
                    {
                        _logger.LogInformation("Starting charge / Minutes after last stop: {minutesAfterLastStop}", minutesAfterLastStop);
                        await StartCharging(chargePoint, vehicle);
                        _data.StartedChargingAt = DateTime.UtcNow;
                    }
                } 
            }
            else if (importedPower > 3 * 230)
            {
                var importingSinceMinutes = (DateTime.UtcNow - _data.LastImportedAt.Value).TotalMinutes;

                if (isCharging)
                {
                    _logger.LogInformation("Stop charging in {Minutes}", Math.Round(10 - importingSinceMinutes, 2));

                    if (importingSinceMinutes > 10)
                    {
                        _logger.LogInformation("Stopping charge / Importing since minutes: {importingSinceMinutes}", importingSinceMinutes);
                        await StopCharging(chargePoint, vehicle);
                    }
                }
            }

            if ((exportedPower > chargePointData.PhaseCount * 230) && chargePointIsCharging)
            {
                int currentCharge = Convert.ToInt32(Math.Round(chargePointData.CurrentPhase1, 0));
                int deltaCharge = Convert.ToInt32(Math.Round((decimal)exportedPower / (chargePointData.PhaseCount * 230), 0));

                if ((deltaCharge + currentCharge > 19) && (DateTime.UtcNow - _data.PhasesChangeTo1At).TotalMinutes > 5)
                {
                    if (chargePointData.PhaseCount != 3)
                    {
                        _data.PhasesChangeTo3At = DateTime.UtcNow;
                        _data.LastCurrentChangeAt = DateTime.UtcNow;
                        _data.LastCurrentDelta = 0;

                        _logger.LogInformation("Setting phases: {phases}", 3);
                        await SetPhases(chargePoint, vehicle, 3);
                    }
                }
                else
                {
                    int newCharge = Math.Min(currentCharge + deltaCharge, 16);

                    if ((currentCharge != newCharge) && (DateTime.UtcNow - _data.LastCurrentChangeAt).TotalSeconds > (_data.LastCurrentDelta * 10))
                    {
                        _data.LastCurrentChangeAt = DateTime.UtcNow;
                        _data.LastCurrentDelta = deltaCharge;

                        _logger.LogInformation("Increase Current: {old} >>> {new}", currentCharge, newCharge);
                        SetCurrent(chargePoint, newCharge);
                    }
                }
            }
            else if ((importedPower > chargePointData.PhaseCount * 230) && chargePointIsCharging)
            {
                int currentCharge = Convert.ToInt32(Math.Round(chargePointData.CurrentPhase1, 0));
                int deltaCharge = Convert.ToInt32(Math.Round((decimal)importedPower / (chargePointData.PhaseCount * 230), 0));
         
                if ((currentCharge - deltaCharge < 6) && (DateTime.UtcNow - _data.PhasesChangeTo3At).TotalMinutes > 5)
                {
                    if (chargePointData.PhaseCount != 1)
                    {
                        _data.PhasesChangeTo1At = DateTime.UtcNow;
                        _data.LastCurrentChangeAt = DateTime.UtcNow;
                        _data.LastCurrentDelta = 0;

                        _logger.LogInformation("Setting phases: {phases}", 1);
                        await SetPhases(chargePoint, vehicle, 1);
                    }
                }
                else
                {
                    int newCharge = Math.Max(currentCharge - deltaCharge, 6);

                    if ((currentCharge != newCharge) && (DateTime.UtcNow - _data.LastCurrentChangeAt).TotalSeconds > (_data.LastCurrentDelta * 10))
                    {
                        _data.LastCurrentChangeAt = DateTime.UtcNow;
                        _data.LastCurrentDelta = deltaCharge;

                        _logger.LogInformation("Decrease Current: {old} >>> {new}", currentCharge, newCharge);
                        SetCurrent(chargePoint, newCharge);
                    }
                }
            }
        }
    }
}
