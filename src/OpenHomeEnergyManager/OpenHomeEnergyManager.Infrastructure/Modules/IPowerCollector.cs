using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules
{
    interface IPowerCollector
    {
        public int CurrentPowerCollected { get; }
    }
}
