using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargePointServices
{
    public class ChargePointService
    {
        private readonly IModuleServiceRegistry _moduleServiceRegistry;

        public ChargePointService(IModuleServiceRegistry moduleServiceRegistry)
        {
            _moduleServiceRegistry = moduleServiceRegistry;
        }

        public ChargePointDataset GetCurrentData(int moduleId)
        {
            var moduleService = _moduleServiceRegistry.FindById(moduleId);

            return new ChargePointDataset()
            {
                Power = moduleService.GetCapability<PowerCapability>("POWER").Value,
                CurrentPhase1 = moduleService.GetCapability<CurrentCapability>("CURRENT_PHASE_1").Value,
                CurrentPhase2 = moduleService.GetCapability<CurrentCapability>("CURRENT_PHASE_2").Value,
                CurrentPhase3 = moduleService.GetCapability<CurrentCapability>("CURRENT_PHASE_3").Value,
                VoltagePhase1 = moduleService.GetCapability<VoltageCapability>("VOLTAGE_PHASE_1").Value,
                VoltagePhase2 = moduleService.GetCapability<VoltageCapability>("VOLTAGE_PHASE_2").Value,
                VoltagePhase3 = moduleService.GetCapability<VoltageCapability>("VOLTAGE_PHASE_3").Value,
                IsPlugged = moduleService.GetCapability<IsPluggedCapability>("IS_PLUGGED").Value,
                IsCharging = moduleService.GetCapability<IsChargingCapability>("IS_CHARGING").Value,
                PhaseCount = moduleService.GetCapability<PhaseCountCapability>("PHASE_COUNT").Value,
            };
        }

        public void SetCurrent(int moduleId, int current)
        {
            var moduleService = _moduleServiceRegistry.FindById(moduleId);
            moduleService.GetCapability<SetCurrentCapability>("SET_CURRENT").SetCurrent(current);
        }

        public void SetPhaseCount(int moduleId, int phases)
        {
            var moduleService = _moduleServiceRegistry.FindById(moduleId);
            moduleService.GetCapability<SetPhaseCountCapability>("SET_PHASE_COUNT").Set(phases);
        }
    }
}
