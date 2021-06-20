namespace OpenHomeEnergyManager.Api.Controllers.V1.Vehicles.Queries
{
    public class VehicleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public int? ModuleId { get; set; }

        public bool ChargerMustBeOffOnChanges { get; set; }
    }
}
