using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities
{
    public class SetIsChargingCapability : Capability
    {
        private Action<bool> _setAction;

        public SetIsChargingCapability(string key, string name, Action<bool> setAction) 
            : base(key, name)
        {
            _setAction = setAction;
        }

        public void Set(bool value)
        {
            _setAction.Invoke(value);
        }
    }
}
