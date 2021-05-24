using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities
{
    public class SetCurrentCapability : Capability
    {
        private Action<int> _currentSetAction;

        public SetCurrentCapability(string key, string name, Action<int> currentSetAction) 
            : base(key, name)
        {
            _currentSetAction = currentSetAction;
        }

        public void SetCurrent(int current)
        {
            _currentSetAction.Invoke(current);
        }
    }
}
