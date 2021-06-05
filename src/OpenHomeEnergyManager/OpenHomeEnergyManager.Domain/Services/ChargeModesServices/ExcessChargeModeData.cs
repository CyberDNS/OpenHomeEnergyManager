using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public class ExcessChargeModeData
    {
        public DateTime StartedChargingAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastImportedAt { get; set; } = null;

        public DateTime StoppedChargingAt { get; set; } = DateTime.MinValue;

        public int Phases { get; set; } = 1;

        public DateTime PhasesChangeTo3At { get; set; } = DateTime.UtcNow;
        public DateTime PhasesChangeTo1At { get; set; } = DateTime.UtcNow;

        public DateTime LastCurrentChangeAt { get; set; } = DateTime.UtcNow;
    }
}
