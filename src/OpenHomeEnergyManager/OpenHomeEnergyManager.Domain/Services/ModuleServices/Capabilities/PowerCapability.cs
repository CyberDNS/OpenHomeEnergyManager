using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities
{
    public class PowerCapability : Capability
    {
        public PowerCapability(string key, string name) : base(key, name)
        {
        }

        public int Power { get; set; }
    }
}
