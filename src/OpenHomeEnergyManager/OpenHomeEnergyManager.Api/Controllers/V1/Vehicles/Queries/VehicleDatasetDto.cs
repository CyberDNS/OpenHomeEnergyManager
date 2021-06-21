namespace OpenHomeEnergyManager.Api.Controllers.V1.Vehicles.Queries
{
    public class VehicleDatasetDto
    {
        public bool IsCharging { get; set; }
        public bool IsChargedToChargeLimit { get; set; }

        public decimal ChargeLimit { get; set; }

        public decimal StateOfCharge { get; set; }
    }
}
