using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using OpenHomeEnergyManager.Blazor.Components.Dialogs;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Queries;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ModuleServiceDefinitions;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ModuleServiceDefinitions.Queries;

namespace OpenHomeEnergyManager.Blazor.Components.ModuleManagement.Settings
{
    public partial class ModuleSettingsCard : ComponentBase
    {
        [Inject] private IDialogService _dialog { get; set; }
        [Inject] private ModulesClient _moduleClient { get; set; }

        [CascadingParameter] public IEnumerable<ModuleServiceDefinitionDto> ModuleServiceDefinitions { get; set; }
        [Parameter] public ModuleDto Module { get; set; }
        [Parameter] public EventHandler<ModuleDto> OnDeleted { get; set; }


        private ModuleSettingsModel _model { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _model = new ModuleSettingsModel()
                {
                    Id = Module.Id,
                    Name = Module.Name,
                    Settings = Module.Settings
                };

                _model.ModuleServiceDefinition = ModuleServiceDefinitions.Where(d => d.Key.Equals(Module.ModuleServiceDefinitionKey)).SingleOrDefault();

                if (_model.ModuleServiceDefinition is not null)
                {
                    foreach (var setting in _model.ModuleServiceDefinition.Settings)
                    {
                        if (!_model.Settings.ContainsKey(setting.Key)) { _model.Settings.Add(setting.Key, ""); }
                    }
                }

                StateHasChanged();
            }
        }

        private void OnModuleServiceDefinitionChanged(ModuleServiceDefinitionDto selectedModuleServiceDefinition)
        {
            _model.Settings = selectedModuleServiceDefinition.Settings.ToDictionary(k => k.Key, k => "");
            _model.ModuleServiceDefinition = selectedModuleServiceDefinition;
        }

        private async Task OnValidSubmit(EditContext context)
        {
            await _moduleClient.UpdateInformationAsync(Module.Id, new UpdateInformationDto()
            {
                Name = _model.Name,
                ModuleServiceDefinitionKey = _model.ModuleServiceDefinitionKey,
                Settings = _model.Settings
            });
        }


        private async Task OnDelete()
        {
            DialogParameters parameters = new DialogParameters();
            parameters.Add("DialogMessage", $"Do you want to delete the module {_model.Name}?");

            var dialog = _dialog.Show<ConfirmationDialog>("Please confirm", parameters: parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await _moduleClient.DeleteAsync(_model.Id);
                OnDeleted.Invoke(this, Module);
            }
        }
    }
}
