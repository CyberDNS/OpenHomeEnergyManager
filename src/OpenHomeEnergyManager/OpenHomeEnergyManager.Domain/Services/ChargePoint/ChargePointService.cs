using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargePoint
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
                ConfiguredCurrent = GetCapability<CurrentCapability>(moduleService, "CURRENT_CONFIGURED").Current,
                Power = GetCapability<PowerCapability>(moduleService, "POWER").Power
            };
        }

        private static T GetCapability<T>(IModuleService moduleService, string key)
            where T: Capability
        {
            var capability =  moduleService.Capabilities
                             .OfType<T>()
                             .Where(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                             .SingleOrDefault();

            return capability;
        }
    }
}
