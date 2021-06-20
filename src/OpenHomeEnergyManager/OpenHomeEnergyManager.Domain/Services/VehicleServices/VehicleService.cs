using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.VehicleServices
{
    public class VehicleService
    {
        private readonly IModuleServiceRegistry _moduleServiceRegistry;

        public VehicleService(IModuleServiceRegistry moduleServiceRegistry)
        {
            _moduleServiceRegistry = moduleServiceRegistry;
        }

        public VehicleDataset GetCurrentData(int moduleId)
        {
            var moduleService = _moduleServiceRegistry.FindById(moduleId);

            return new VehicleDataset()
            {
                IsCharging = moduleService.GetCapability<IsChargingCapability>("IS_CHARGING").Value,
                IsChargedToChargeLimit = moduleService.GetCapability<IsChargedToChargeLimitCapability>("IS_CHARGED_TO_CHARGE_LIMIT").Value,
                ChargeLimit = moduleService.GetCapability<ChargeLimitCapability>("CHARGE_LIMIT").Value,
                StateOfCharge = moduleService.GetCapability<StateOfChargeCapability>("SOC").Value,
            };
        }

        public void SetIsCharging(int moduleId, bool turnOn)
        {
            var hassModuleService = _moduleServiceRegistry.FindById(moduleId);
            hassModuleService.GetCapability<SetIsChargingCapability>("SET_IS_CHARGING").Set(turnOn);
        }
    }
}
