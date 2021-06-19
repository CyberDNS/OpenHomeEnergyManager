using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos;

namespace OpenHomeEnergyManager.Blazor.Pages.ModuleManagement.Settings
{
    public partial class ModulesSettings : ComponentBase
    {
        [Inject] private ModuleClient _moduleClient { get; set; }

        private IList<ModuleDto> _modules;

        private string _restartButton = "Restart";

        protected override async Task OnParametersSetAsync()
        {
            _modules = (await _moduleClient.GetAllAsync()).ToList();

            await Task.CompletedTask;
        }

        private void AddModule()
        {
            _modules.Add(new ModuleDto());
        }

        public void OnModuleRemoved(object target, ModuleDto module)
        {
            _modules.Remove(module);
            StateHasChanged();
        }

        public async Task OnRestart()
        {
            _restartButton = "Restarting...";
            StateHasChanged();

            await _moduleClient.RestartModules();

            _restartButton = "Restart";
            StateHasChanged();
        }
    }
}
