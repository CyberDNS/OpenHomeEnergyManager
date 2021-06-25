using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public interface IRuleBasedChargeMode : IChargeMode
    {

        public bool EvaluateRule();
    }
}
