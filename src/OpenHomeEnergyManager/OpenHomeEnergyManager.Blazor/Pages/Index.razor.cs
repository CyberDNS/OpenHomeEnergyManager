using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace OpenHomeEnergyManager.Blazor.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] private NavigationManager _navigationManager { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _navigationManager.NavigateTo("dashboard");
            }
        }
    }
}
