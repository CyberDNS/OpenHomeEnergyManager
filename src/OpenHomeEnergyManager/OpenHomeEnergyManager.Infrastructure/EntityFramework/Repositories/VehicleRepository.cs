using Microsoft.EntityFrameworkCore;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.EntityFramework.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly OpenHomeEnergyManagerDbContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public VehicleRepository(OpenHomeEnergyManagerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public Vehicle Add(Vehicle vehicle)
        {
            return _context.Vehicles.Add(vehicle).Entity;
        }

        public async Task<Vehicle> FindByIdAsync(int id)
        {
            var vehicle = await _context
                                .Vehicles
                                .SingleOrDefaultAsync(c => c.Id == id);

            return vehicle;
        }

        public IEnumerable<Vehicle> GetAll()
        {
            var vehicles = _context
                    .Vehicles
                    .AsEnumerable();

            return vehicles.ToArray();
        }

        public void Remove(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
        }
    }
}
