using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public class DirectChargeMode : IChargeMode
    {
        public string UniqueIdentifier => "DIRECT";

        public Task LoopAsync(ChargePoint chargePoint, ChargePointService chargePointService)
        {
            var isCharging = chargePointService.GetCurrentData(chargePoint.ModuleId).IsCharging;

            if (!isCharging)
            {
                chargePointService.SetCurrent(chargePoint.ModuleId, 10);
                chargePointService.SetIsCharging(true);
            }

            return Task.CompletedTask;
        }
    }
}
