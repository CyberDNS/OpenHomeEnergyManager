using OpenHomeEnergyManager.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Model.VehicleAggregate
{
    public class Vehicle : Entity, IAggregateRoot
    {
        public string Name { get; set; }

        public string Image { get; private set; }

        public int? ModuleId { get; set; }

        public bool ChargerMustBeOffOnChanges { get; set; }

        public Vehicle(string name)
        {
            Name = name;
        }
    }
}
