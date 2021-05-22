using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities
{
    public class CurrentCapability : Capability
    {
        public CurrentCapability(string key, string name) : base(key, name)
        {
        }

        public int Current { get; set; }
    }
}
