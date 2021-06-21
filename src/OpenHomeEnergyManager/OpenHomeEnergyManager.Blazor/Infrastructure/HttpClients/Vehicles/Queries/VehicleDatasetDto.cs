using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Queries
{
    public class VehicleDatasetDto
    {
        public bool IsCharging { get; set; }
        public bool IsChargedToChargeLimit { get; set; }

        public decimal ChargeLimit { get; set; }

        public decimal StateOfCharge { get; set; }
    }
}
