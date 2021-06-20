using System.Collections.Generic;

namespace OpenHomeEnergyManager.Api.Controllers.V1.ModuleServiceDefinitions.Queries
{
    public class ModuleServiceDefinitionDto
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
