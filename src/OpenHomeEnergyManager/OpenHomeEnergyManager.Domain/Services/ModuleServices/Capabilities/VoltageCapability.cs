using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities
{
    public class VoltageCapability : Capability
    {
        public VoltageCapability(string key, string name) : base(key, name)
        {
        }

        public decimal Value { get; set; }
    }
}
