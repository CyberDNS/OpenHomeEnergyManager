using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules
{
    public interface IPowerProvider
    {
        public int CurrentPowerProvided { get; }
    }
}
