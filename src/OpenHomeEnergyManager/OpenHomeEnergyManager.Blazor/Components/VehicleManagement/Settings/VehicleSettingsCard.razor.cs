using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using OpenHomeEnergyManager.Blazor.Components.ChargePointManagement.Settings;
using OpenHomeEnergyManager.Blazor.Components.Dialogs;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Queries;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Queries;

namespace OpenHomeEnergyManager.Blazor.Components.VehicleManagement.Settings
{
    public partial class VehicleSettingsCard : ComponentBase
    {
        [Inject] private IDialogService _dialogService { get; set; }
        [Inject] private VehiclesClient _vehiclesClient { get; set; }


        [CascadingParameter] public IEnumerable<ModuleDto> Modules { get; set; }
        [Parameter] public VehicleDto Vehicle { get; set; }
        [Parameter] public EventHandler<VehicleDto> OnDeleted { get; set; }

        private VehicleSettingsModel _model;

        protected override void OnParametersSet()
        {
            _model = new VehicleSettingsModel()
            {
                Name = Vehicle.Name,
                ModuleId = Vehicle.ModuleId,
                Image = Vehicle.Image,
                ChargerMustBeOffOnChanges = Vehicle.ChargerMustBeOffOnChanges
            };

            _model.Module = Modules.Where(d => d.Id.Equals(Vehicle.ModuleId)).SingleOrDefault();
        }

        private async Task OnValidSubmit(EditContext context)
        {
            await _vehiclesClient.UpdateInformationAsync(Vehicle.Id, new UpdateInformationDto()
            {
                Name = _model.Name,
                ModuleId = _model.ModuleId
            });
        }

        private void OnModuleChanged(ModuleDto selectedModule)
        {
            _model.Module = selectedModule;
        }

        private async Task OnDelete()
        {
            DialogParameters parameters = new DialogParameters();
            parameters.Add("DialogMessage", $"Do you want to delete the charge point {Vehicle.Name}?");

            var dialog = _dialogService.Show<ConfirmationDialog>("Please confirm", parameters: parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await _vehiclesClient.DeleteAsync(Vehicle.Id);
                OnDeleted.Invoke(this, Vehicle);
            }
        }
    }
}
