using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public interface IChargeModesService
    {
        Task LoopAsync();
    }
}