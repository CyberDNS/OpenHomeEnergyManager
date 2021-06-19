using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using OpenHomeEnergyManager.Blazor.Components.Dialogs;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos;

namespace OpenHomeEnergyManager.Blazor.Components.ChargePointManagement.Settings
{
    public partial class ChargePointSettingsCard : ComponentBase
    {
        [Inject] private ChargePointClient _chargePointClient { get; set; }

        [Parameter] public ChargePointDto Model { get; set; }

        [Parameter] public EventHandler<ChargePointDto> OnChargePointRemoved { get; set; }

        private bool _isNew => Model.Id == default(Int32);

        private async Task OnValidSubmit(EditContext context)
        {
            if (_isNew)
            {
                int id = (await _chargePointClient.AddAsync(Model)).Id;
                Model.Id = id;
            }
            else
            {
                await _chargePointClient.UpdateAsync(Model);
            }
        }

        private void OnAddCanceled()
        {
            OnChargePointRemoved.Invoke(this, Model);
        }

        private async Task OnDelete()
        {
            DialogParameters parameters = new DialogParameters();
            parameters.Add("DialogMessage", $"Do you want to delete the charge point {Model.Name}?");

            var dialog = Dialog.Show<ConfirmationDialog>("Please confirm", parameters: parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await _chargePointClient.DeleteAsync(Model);
                OnChargePointRemoved.Invoke(this, Model);
            }
        }
    }
}
