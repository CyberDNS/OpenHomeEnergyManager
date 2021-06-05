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
    public class DirectChargeMode : IChargeMode
    {
        private readonly ChargePointService _chargePointService;
        private readonly VehicleService _vehicleService;

        public string UniqueIdentifier => "DIRECT";

        public DirectChargeMode(ChargePointService chargePointService, VehicleService vehicleService)
        {
            _chargePointService = chargePointService;
            _vehicleService = vehicleService;
        }

        public async Task LoopAsync(ChargePoint chargePoint, Vehicle vehicle)
        {
            var isCharging = _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsCharging;

            //if ((isNotCharging) && (minutesAfterLastStop > 5))
            //{
            //    _logger.LogInformation("Starting charge / Minutes after last stop: {minutesAfterLastStop}", minutesAfterLastStop);
            //    await StartCharging(chargePoint, vehicle);
            //    _logger.LogInformation("Started!");
            //}
        }

        //private async Task StartCharging(ChargePoint chargePoint, Vehicle vehicle)
        //{
        //    _chargePointService.SetCurrent(chargePoint.ModuleId.Value, 6);
        //    //_chargePointService.SetPhaseCount(chargePoint.ModuleId.Value, 1);

        //    if (vehicle.ChargerMustBeOffOnChanges)
        //    {
        //        await Task.Delay(TimeSpan.FromSeconds(1));
        //        _vehicleService.SetIsCharging(vehicle.ModuleId, true);
        //    }

        //    var result = Policy.HandleResult<bool>(b => b)
        //        .WaitAndRetry(3, r => TimeSpan.FromSeconds(5))
        //        .ExecuteAndCapture(() => GetIsCharging(chargePoint, vehicle));

        //    if (result.Outcome == OutcomeType.Failure) { throw new ApplicationException("Car did not start charging"); }
        //}
    }
}
