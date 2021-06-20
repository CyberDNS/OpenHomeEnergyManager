using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Extensions;
using MudBlazor;
using OpenHomeEnergyManager.Blazor.Components.Dialogs;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Queries;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Images;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Queries;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles;

namespace OpenHomeEnergyManager.Blazor.Components.ChargePointManagement.Settings
{
    public partial class ChargePointSettingsCard : ComponentBase
    {
        [Inject] private IDialogService _dialogService { get; set; }
        [Inject] private ChargePointsClient _chargePointClient { get; set; }
        [Inject] private ImagesClient _imagesClient { get; set; }
        [Inject] private NavigationManager _navigationManager { get; set; }

        [CascadingParameter] public IEnumerable<ModuleDto> Modules { get; set; }
        [Parameter] public ChargePointDto ChargePoint { get; set; }
        [Parameter] public EventHandler<ChargePointDto> OnDeleted { get; set; }

        private ChargePointSettingsModel _model;
        private IBrowserFile _file;

        protected override void OnParametersSet()
        {
            _model = new ChargePointSettingsModel()
            {
                Name = ChargePoint.Name,
                ModuleId = ChargePoint.ModuleId,
                Image = ChargePoint.Image
            };

            _model.Module = Modules.Where(d => d.Id.Equals(ChargePoint.ModuleId)).SingleOrDefault();
        }

        private async Task OnValidSubmit(EditContext context)
        {
            await _chargePointClient.UpdateInformationAsync(ChargePoint.Id, new UpdateInformationDto()
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
            parameters.Add("DialogMessage", $"Do you want to delete the charge point {ChargePoint.Name}?");

            var dialog = _dialogService.Show<ConfirmationDialog>("Please confirm", parameters: parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await _chargePointClient.DeleteAsync(ChargePoint.Id);
                OnDeleted.Invoke(this, ChargePoint);
            }
        }


        private async Task UploadFile(InputFileChangeEventArgs e)
        {
            var extension = Path.GetExtension(e.File.Name);
            await _imagesClient.UploadAsync(e.File, $"chargepoint_{ChargePoint.Id}{extension}");

            ChargePoint = await _chargePointClient.GetAsync(ChargePoint.Id);

            StateHasChanged();
        }
    }
}
