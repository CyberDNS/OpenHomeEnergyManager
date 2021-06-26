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
    public abstract class TargetChargeModeBase : ChargeModeBase, IRuleBasedChargeMode
    {

        protected decimal _targetSoc;

        protected TargetChargeModeBase(ILogger<ChargeModeBase> logger, ChargePointService chargePointService, VehicleService vehicleService) 
            : base(logger, chargePointService, vehicleService)
        {
        }

        public abstract bool EvaluateRule(ChargePoint chargePoint, Vehicle vehicle);
    }
}
