using OpenHomeEnergyManager.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Model.ModuleAggregate
{
    public class Module : Entity, IAggregateRoot
    {
        public string Name { get; set; }

        public string ModuleServiceDefinitionKey { get; set; }

        public Dictionary<string, string> Settings { get; set; }

        public Module(string name)
        {
            Name = name;
        }
    }
}
