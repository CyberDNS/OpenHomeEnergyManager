using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.SmaTriPower
{
    class SmaTriPowerServiceDefinition : IModuleServiceDefinition
    {
        public string Key => "64948430-E514-4DFE-AE5B-95C89B98A6AE";
        public string Name => "SMA Sunny TriPower by Modbus";
        public string Description => "Connect to your SMA Sunny TriPower to get the current power provided from your photovoltaic system";
        public Type Type => typeof(SmaTriPowerService);

        public IDictionary<string, string> Settings => new Dictionary<string, string>() { { "IP Address", "text" } };
    }
}
