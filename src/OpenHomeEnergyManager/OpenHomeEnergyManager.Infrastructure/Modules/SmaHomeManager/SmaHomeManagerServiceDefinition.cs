using OpenHomeEnergyManager.Domain.Model.ModuleServiceDefinitionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.SmaHomeManager
{
    class SmaHomeManagerServiceDefinition : IModuleServiceDefinition
    {
        public string Key => "0E133445-591A-4BF8-9010-D893F099C1B7";
        public string Name => "SMA Sunny Home Manager 2.0";
        public string Description => "Connect you SMA Sunny Home Manager 2.0 and get values for the current power provided or current power collected";
        public Type Type => typeof(SmaHomeManagerService);

        public IDictionary<string, string> Settings => new Dictionary<string, string>() { { "Serial Number", "text" } };
    }
}
