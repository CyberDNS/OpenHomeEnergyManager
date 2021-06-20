using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Queries
{
    public class ModuleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ModuleServiceDefinitionKey { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
