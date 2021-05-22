using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.OpenWb
{
    class OpenWbServiceDefinition : IModuleServiceDefinition
    {
        public string Key => "4F86A2EE-D0AE-4F7E-B309-901A85C91280";
        public string Name => "OpenWb Wallbox by MQTT";
        public string Description => "Connect your OpenWb Wallbox via the MQTT Broker that is running on the OpenWb Wallbox";
        public Type Type => typeof(OpenWbService);

        public IDictionary<string, string> Settings => new Dictionary<string, string>() { { "IP Address", "text" } };
    }
}
