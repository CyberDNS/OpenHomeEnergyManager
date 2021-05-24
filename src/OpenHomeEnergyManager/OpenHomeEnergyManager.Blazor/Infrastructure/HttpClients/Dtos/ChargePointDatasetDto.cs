using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos
{
    public class ChargePointDatasetDto
    {
        public int Power { get; set; }

        public decimal CurrentPhase1 { get; set; }
        public decimal CurrentPhase2 { get; set; }
        public decimal CurrentPhase3 { get; set; }

        public decimal VoltagePhase1 { get; set; }
        public decimal VoltagePhase2 { get; set; }
        public decimal VoltagePhase3 { get; set; }

        public int PhaseCount { get; set; }

        public bool IsCharging { get; set; }
        public bool IsPlugged { get; set; }
    }
}
