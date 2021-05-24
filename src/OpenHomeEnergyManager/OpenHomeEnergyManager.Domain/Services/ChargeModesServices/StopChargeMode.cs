using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public class StopChargeMode : IChargeMode
    {
        public string UniqueIdentifier => "STOP";

        public Task LoopAsync(ChargePoint chargePoint, ChargePointService chargePointService)
        {
            var isCharging = chargePointService.GetCurrentData(chargePoint.ModuleId).IsCharging;

            if (isCharging)
            {
                chargePointService.SetIsCharging(false);
                chargePointService.SetCurrent(chargePoint.ModuleId, 0);
            }

            return Task.CompletedTask;
        }
    }
}
