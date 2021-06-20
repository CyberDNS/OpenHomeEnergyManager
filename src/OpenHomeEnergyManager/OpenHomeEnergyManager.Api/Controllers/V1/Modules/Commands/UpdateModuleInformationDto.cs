using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Modules.Commands
{
    public class UpdateModuleInformationDto
    {
        public string Name { get; set; }

        public string ModuleServiceDefinitionKey { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
