using OpenHomeEnergyManager.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Model.VehicleAggregate
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Vehicle Add(Vehicle vehicle);

        void Remove(Vehicle vehicle);

        IEnumerable<Vehicle> GetAll();

        Task<Vehicle> FindByIdAsync(int id);
    }
}
}
