using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate
{
    public interface IModuleServiceDefinition
    {
        public string Key { get; }
        public string Name { get; }
        public string Description { get; }

        public Type Type { get; }

        public IDictionary<string, string> Settings { get; }

    }
}
