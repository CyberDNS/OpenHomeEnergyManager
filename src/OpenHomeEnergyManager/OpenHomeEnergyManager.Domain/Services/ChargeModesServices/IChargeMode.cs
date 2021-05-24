using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public interface IChargeMode
    {
        public string UniqueIdentifier { get; }

        public Task LoopAsync(ChargePoint chargePoint, ChargePointService chargePointService);
    }
}
