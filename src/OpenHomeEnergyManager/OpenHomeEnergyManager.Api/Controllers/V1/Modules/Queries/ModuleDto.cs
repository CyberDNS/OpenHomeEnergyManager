using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenHomeEnergyManager.Api.Controllers.V1.Modules.Queries
{
    public class ModuleDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ModuleServiceDefinitionKey { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
