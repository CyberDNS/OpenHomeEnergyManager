using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using OpenHomeEnergyManager.Blazor.Components.Dialogs;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Queries;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Queries;

namespace OpenHomeEnergyManager.Blazor.Pages.VehicleManagement.Settings
{
    public partial class VehiclesSettings : ComponentBase
    {
        [Inject] private IDialogService _dialogService { get; set; }
        [Inject] private VehiclesClient _vehiclesClient { get; set; }
        [Inject] private ModulesClient _modulesClient { get; set; }

        private IList<VehicleDto> _vehicles = null;
        private IEnumerable<ModuleDto> _modules;

        protected override async Task OnParametersSetAsync()
        {
            _vehicles = (await _vehiclesClient.GetAllAsync()).ToList();
            _modules = await _modulesClient.GetAllAsync();
        }

        private async Task AddVehicle()
        {
            var dialog = _dialogService.Show<AddNamedItemDialog>("Add vehicle");
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await _vehiclesClient.AddAsync(new AddVehicleDto() { Name = result.Data.As<string>() });
            }

            _vehicles = (await _vehiclesClient.GetAllAsync()).ToList();
            StateHasChanged();
        }

        public void OnVehicleDeleted(object target, VehicleDto vehicle)
        {
            _vehicles.Remove(vehicle);
            StateHasChanged();
        }
    }
}
