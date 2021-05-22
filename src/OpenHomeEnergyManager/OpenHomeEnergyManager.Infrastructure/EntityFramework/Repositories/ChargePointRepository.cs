using Microsoft.EntityFrameworkCore;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.EntityFramework.Repositories
{
    public class ChargePointRepository : IChargePointRepository
    {
        private readonly OpenHomeEnergyManagerDbContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public ChargePointRepository(OpenHomeEnergyManagerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public ChargePoint Add(ChargePoint chargePoint)
        {
            return _context.ChargePoints.Add(chargePoint).Entity;
        }

        public async Task<ChargePoint> FindByIdAsync(int id)
        {
            var chargePoint = await _context
                                .ChargePoints
                                .SingleOrDefaultAsync(c => c.Id == id);

            return chargePoint;
        }

        public IEnumerable<ChargePoint> GetAll()
        {
            var chargePoints = _context
                    .ChargePoints
                    .AsEnumerable();

            return chargePoints.ToArray();
        }

        public void Remove(ChargePoint chargePoint)
        {
            _context.ChargePoints.Remove(chargePoint);
        }

        public ChargePoint Update(ChargePoint chargePoint)
        {
            _context.Entry(chargePoint).State = EntityState.Modified;

            return chargePoint;
        }
    }
}
