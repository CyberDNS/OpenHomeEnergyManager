using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities
{
    public class IsChargingCapability : Capability
    {
        public IsChargingCapability(string key, string name) : base(key, name)
        {
        }

        public bool Value { get; set; }
    }
}
