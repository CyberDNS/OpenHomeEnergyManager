using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos
{
    public class ModuleDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        private ModuleServiceDefinitionDto _moduleServiceDefinition;
        [JsonIgnore]
        public ModuleServiceDefinitionDto ModuleServiceDefinition
        {
            get { return _moduleServiceDefinition; }
            set
            {
                _moduleServiceDefinition = value;
                ModuleServiceDefinitionKey = _moduleServiceDefinition?.Key;
            }
        }

        public string ModuleServiceDefinitionKey { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
