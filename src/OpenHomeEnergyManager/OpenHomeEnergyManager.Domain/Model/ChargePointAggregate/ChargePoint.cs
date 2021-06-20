using OpenHomeEnergyManager.Domain.SeedWork;
using OpenHomeEnergyManager.Domain.Services.ChargeModesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Model.ChargePointAggregate
{
    public class ChargePoint : Entity, IAggregateRoot
    {
        public string Name { get; set; }

        public string Image { get; private set; }

        public int? ModuleId { get; set; } 

        public int? VehicleId { get; set; }

        public ChargeModes CurrentChargeMode { get; set; }

        public ChargePoint(string name)
        {
            Name = name;
        }
    }
}
  