using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using OpenHomeEnergyManager.Blazor.Components.Dialogs;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Queries;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Queries;

namespace OpenHomeEnergyManager.Blazor.Pages.ChargePointManagement.Settings
{
    public partial class ChargePointsSettings : ComponentBase
    {
        [Inject] private IDialogService _dialogService { get; set; }
        [Inject] private ChargePointsClient _chargePointClient { get; set; }
        [Inject] private ModulesClient _modulesClient { get; set; }

        private IList<ChargePointDto> _chargePoints = null;
        private IEnumerable<ModuleDto> _modules;
 
        protected override async Task OnParametersSetAsync()
        {
            _chargePoints = (await _chargePointClient.GetAllAsync()).ToList();
            _modules = await _modulesClient.GetAllAsync();
        }

        private async Task AddChargePoint()
        {
            var dialog = _dialogService.Show<AddNamedItemDialog>("Add charge point");
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await _chargePointClient.AddAsync(new AddChargePointDto() { Name = result.Data.As<string>() });
            }

            _chargePoints = (await _chargePointClient.GetAllAsync()).ToList();
            StateHasChanged();
        }

        public void OnChargePointDeleted(object target, ChargePointDto chargePoint)
        {
            _chargePoints.Remove(chargePoint);
            StateHasChanged();
        }
    }
}
