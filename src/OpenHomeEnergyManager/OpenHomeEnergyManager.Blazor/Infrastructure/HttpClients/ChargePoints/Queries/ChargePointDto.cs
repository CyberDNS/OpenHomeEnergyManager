using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Shared;

namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Queries
{
    public class ChargePointDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public int? ModuleId { get; set; }
        public int? VehicleId { get; set; }

        public ChargeModesDto CurrentChargeMode { get; set; }
    }
}
