using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using OpenHomeEnergyManager.Blazor.Components.Dialogs;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Modules.Queries;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ModuleServiceDefinitions;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ModuleServiceDefinitions.Queries;

namespace OpenHomeEnergyManager.Blazor.Pages.ModuleManagement.Settings
{
    public partial class ModulesSettings : ComponentBase
    {
        [Inject] private IDialogService _dialogService { get; set; }
        [Inject] private ModulesClient _moduleClient { get; set; }
        [Inject] private ModuleServiceDefinitionsClient _moduleServiceDefinitionClient { get; set; }


        private IList<ModuleDto> _modules;
        private IEnumerable<ModuleServiceDefinitionDto> _moduleServiceDefinitions;

        protected override async Task OnParametersSetAsync()
        {
            _modules = (await _moduleClient.GetAllAsync()).ToList();
            _moduleServiceDefinitions = await _moduleServiceDefinitionClient.GetAllAsync();

            await Task.CompletedTask;
        }

        private async Task AddModule()
        {
            var dialog = _dialogService.Show<AddNamedItemDialog>("Add module");
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await _moduleClient.AddAsync(new AddModuleDto() { Name = result.Data.As<string>() });
            }

            _modules = (await _moduleClient.GetAllAsync()).ToList();
            StateHasChanged();
        }

        public void OnModuleDeleted(object target, ModuleDto module)
        {
            _modules.Remove(module);
            StateHasChanged();
        }
    }
}
