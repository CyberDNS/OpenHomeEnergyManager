namespace OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Commands
{
    public class UpdateInformationDto
    {
        public string Name { get; set; }

        public int? ModuleId { get; set; }

        public bool ChargerMustBeOffOnChanges { get; set; }

    }
}
