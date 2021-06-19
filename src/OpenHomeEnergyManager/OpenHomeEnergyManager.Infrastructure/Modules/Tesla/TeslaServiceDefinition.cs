using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using OpenHomeEnergyManager.Infrastructure.Modules.HomeAssistant;

namespace OpenHomeEnergyManager.Infrastructure.Modules.Tesla
{
    public class TeslaServiceDefinition : IModuleServiceDefinition
    {
        public string Key => "BE9C10C1-3B43-4927-8EE3-AA833D0EBFCD";
        public string Name => "Tesla";
        public string Description => "Integrate your Tesla by Tesla API";
        public Type Type => typeof(TeslaService);

        public IDictionary<string, string> Settings => new Dictionary<string, string>()
        {
            { "E-Mail", "text" },
            { "Password", "text" }
        };
    }
}
