using System.Collections.Generic;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ModuleServiceDefinitions.Queries
{
    public class ModuleServiceDefinitionDto
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
