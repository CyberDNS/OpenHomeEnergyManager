using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public interface IRuleBasedChargeMode : IChargeMode
    {

        public bool EvaluateRule(ChargePoint chargePoint, Vehicle vehicle);
    }
}
