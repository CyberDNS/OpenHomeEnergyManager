using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public interface IChargeModesService
    {
        Task LoopAsync(ChargePoint chargePoint);
    }
}