using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities
{
    public class SetPhaseCountCapability : Capability
    {
        private Action<int> _setAction;

        public SetPhaseCountCapability(string key, string name, Action<int> setAction) 
            : base(key, name)
        {
            _setAction = setAction;
        }

        public void Set(int value)
        {
            _setAction.Invoke(value);
        }
    }
}
