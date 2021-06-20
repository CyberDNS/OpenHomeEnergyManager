using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.VehicleServices
{
    public class VehicleDataset
    {
        public bool IsCharging { get; set; }
        public bool IsChargedToChargeLimit { get; set; }

        public decimal ChargeLimit { get; set; }

        public decimal StateOfCharge { get; set; }
    }
}
