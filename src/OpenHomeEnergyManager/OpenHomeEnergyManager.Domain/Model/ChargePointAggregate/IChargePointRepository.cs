using OpenHomeEnergyManager.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Model.ChargePointAggregate
{
    public interface IChargePointRepository : IRepository<ChargePoint>
    {
        ChargePoint Add(ChargePoint chargePoint);
        ChargePoint Update(ChargePoint chargePoint);

        void Remove(ChargePoint chargePoint);

        IEnumerable<ChargePoint> GetAll();

        Task<ChargePoint> FindByIdAsync(int id);
    }
}
