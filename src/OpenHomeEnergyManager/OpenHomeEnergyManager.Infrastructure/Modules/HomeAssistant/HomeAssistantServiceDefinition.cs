using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.HomeAssistant
{
    class HomeAssistantServiceDefinition : IModuleServiceDefinition
    {
        public string Key => "36B93C9E-EBD4-47DE-B4F3-E8F69038790F";
        public string Name => "Home Assistant";
        public string Description => "Get states from entities and make service calls in Home Assistant";
        public Type Type => typeof(HomeAssistantService);

        public IDictionary<string, string> Settings => new Dictionary<string, string>() 
        { 
            { "Home Assistant URL", "text" },
            { "Long Lived Token", "text" }
        };
    }
}
