using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using OpenHomeEnergyManager.Infrastructure.Modules.HomeAssistant;
using OpenHomeEnergyManager.Infrastructure.Modules.OpenWb;
using OpenHomeEnergyManager.Infrastructure.Modules.SmaHomeManager;
using OpenHomeEnergyManager.Infrastructure.Modules.SmaTriPower;
using OpenHomeEnergyManager.Infrastructure.Modules.Tesla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules
{
    class ModuleServiceDefinitionRepository : IModuleServiceDefinitionRepository
    {
        private IEnumerable<IModuleServiceDefinition> _moduleServiceDefinitions;

        public ModuleServiceDefinitionRepository()
        {
            _moduleServiceDefinitions = new IModuleServiceDefinition[]
            {
                new SmaHomeManagerServiceDefinition(),
                //new SmaTriPowerServiceDefinition(),
                //new OpenWbServiceDefinition(),
                //new TeslaHomeAssistantServiceDefinition(),
                //new TeslaServiceDefinition()
            };
        }

        public IEnumerable<IModuleServiceDefinition> GetAll()
        {
            return _moduleServiceDefinitions;
        }
    }
}
