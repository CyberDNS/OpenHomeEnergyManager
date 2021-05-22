using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos
{
    public class ModuleServiceDefinitionDto
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
