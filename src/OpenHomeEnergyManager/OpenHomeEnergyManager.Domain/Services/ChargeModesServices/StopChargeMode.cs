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
    public class StopChargeMode : IChargeMode
    {
        private readonly ILogger _logger;
        private readonly ChargePointService _chargePointService;
        private readonly VehicleService _vehicleService;

        public string UniqueIdentifier => "STOP";

        public StopChargeMode(ILogger<StopChargeMode> logger, ChargePointService chargePointService, VehicleService vehicleService)
        {
            _logger = logger;
            _chargePointService = chargePointService;
            _vehicleService = vehicleService;
        }

        public async Task LoopAsync(ChargePoint chargePoint, Vehicle vehicle)
        {
            var isCharging = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsCharging;

            //if (isCharging)
            //{
            //    _logger.LogInformation("Stopping charge...");
            //    await StopCharging(chargePoint, vehicle);
            //    _logger.LogInformation("Stopped!");
            //}
        }

        //private async Task StopCharging(ChargePoint chargePoint, Vehicle vehicle)
        //{
        //    if (vehicle.ChargerMustBeOffOnChanges)
        //    {
        //        _vehicleService.SetIsCharging(vehicle.ModuleId, false);
        //        await Task.Delay(TimeSpan.FromSeconds(1));

        //        var result = Policy.HandleResult<bool>(b => b)
        //            .WaitAndRetry(3, r => TimeSpan.FromSeconds(5))
        //            .ExecuteAndCapture(() => GetIsNotCharging(chargePoint, vehicle));

        //        if (result.Outcome == OutcomeType.Failure) { throw new ApplicationException("Car did not start charging"); }
        //    }

        //    _chargePointService.SetCurrent(chargePoint.ModuleId.Value, 0);
        //}
    }
}
