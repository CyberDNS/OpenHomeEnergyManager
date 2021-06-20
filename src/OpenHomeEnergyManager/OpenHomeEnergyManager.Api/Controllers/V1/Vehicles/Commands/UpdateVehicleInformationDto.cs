namespace OpenHomeEnergyManager.Api.Controllers.V1.Vehicles.Commands
{
    public class UpdateVehicleInformationDto
    {
        public string Name { get; set; }

        public int? ModuleId { get; set; }

        public bool ChargerMustBeOffOnChanges { get; set; }
    }
}
