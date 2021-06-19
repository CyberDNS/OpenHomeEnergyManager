using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace OpenHomeEnergyManager.Blazor.Components.Dialogs
{
    public partial class ConfirmationDialog : ComponentBase
    {
        [Parameter] public string DialogMessage { get; set; }

        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

        void Submit() => MudDialog.Close(DialogResult.Ok(true));
        void Cancel() => MudDialog.Cancel();
    }
}
