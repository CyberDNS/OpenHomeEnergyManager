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

namespace OpenHomeEnergyManager.Blazor.Components.ModuleManagement.Settings
{
    public partial class ModuleSettingsCard : ComponentBase
    {
        [Inject] private IDialogService _dialog { get; set; }
        [Inject] private ModuleClient _moduleClient { get; set; }
        [Inject] private ModuleServiceDefinitionClient _moduleServiceDefinitionClient { get; set; }


        [Parameter] public ModuleDto Model { get; set; }
        [Parameter] public EventHandler<ModuleDto> OnModuleRemoved { get; set; }

        private IEnumerable<ModuleServiceDefinitionDto> _moduleServiceDefinitions { get; set; }
        private bool _isNew => Model.Id == default(Int32);


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _moduleServiceDefinitions = await _moduleServiceDefinitionClient.GetAllAsync();
                Model.ModuleServiceDefinition = _moduleServiceDefinitions.Where(d => d.Key.Equals(Model.ModuleServiceDefinitionKey)).SingleOrDefault();

                if (Model.ModuleServiceDefinition is not null)
                {
                    foreach (var setting in Model.ModuleServiceDefinition.Settings)
                    {
                        if (!Model.Settings.ContainsKey(setting.Key)) { Model.Settings.Add(setting.Key, ""); }
                    }
                }

                StateHasChanged();
            }
        }

        private void OnModuleServiceDefinitionChanged(ModuleServiceDefinitionDto selectedModuleServiceDefinition)
        {
            Model.Settings = selectedModuleServiceDefinition.Settings.ToDictionary(k => k.Key, k => "");
            Model.ModuleServiceDefinition = selectedModuleServiceDefinition;
        }

        private async Task OnValidSubmit(EditContext context)
        {
            if (_isNew)
            {
                int id = (await _moduleClient.AddAsync(Model)).Id;
                Model.Id = id;
            }
            else
            {
                await _moduleClient.UpdateAsync(Model);
            }
        }

        private void OnAddCanceled()
        {
            OnModuleRemoved.Invoke(this, Model);
        }

        private async Task OnDelete()
        {
            DialogParameters parameters = new DialogParameters();
            parameters.Add("DialogMessage", $"Do you want to delete the module {Model.Name}?");

            var dialog = _dialog.Show<ConfirmationDialog>("Please confirm", parameters: parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await _moduleClient.DeleteAsync(Model);
                OnModuleRemoved.Invoke(this, Model);
            }
        }
    }
}
