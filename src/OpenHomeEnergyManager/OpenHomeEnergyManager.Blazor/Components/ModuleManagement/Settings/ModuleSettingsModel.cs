using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ModuleServiceDefinitions.Queries;

namespace OpenHomeEnergyManager.Blazor.Components.ModuleManagement.Settings
{
    public class ModuleSettingsModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string ModuleServiceDefinitionKey { get; set; }

        private ModuleServiceDefinitionDto _moduleServiceDefinition;
        public ModuleServiceDefinitionDto ModuleServiceDefinition
        {
            get => _moduleServiceDefinition;
            set
            {
                _moduleServiceDefinition = value;
                if (_moduleServiceDefinition is not null) { ModuleServiceDefinitionKey = _moduleServiceDefinition.Key; }
            }
        }

        public Dictionary<string, string> Settings { get; set; }
    }
}
