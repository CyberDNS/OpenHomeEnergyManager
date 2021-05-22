using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos
{
    public class ChargePointDatasetDto
    {
        public int Power { get; set; }

        public int ConfiguredCurrent { get; set; }
    }
}
