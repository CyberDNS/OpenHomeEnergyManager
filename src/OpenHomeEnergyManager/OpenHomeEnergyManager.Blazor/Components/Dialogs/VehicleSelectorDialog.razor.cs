using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Queries;

namespace OpenHomeEnergyManager.Blazor.Components.Dialogs
{
    public partial class VehicleSelectorDialog : ComponentBase
    {
        [Inject] private VehiclesClient _vehiclesClient { get; set; }

        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

        private IEnumerable<VehicleDto> _vehicles;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _vehicles = await _vehiclesClient.GetAllAsync();

                StateHasChanged();
            }
        }

        void Submit(int? vehicleId) => MudDialog.Close(DialogResult.Ok(vehicleId));
    }
}

