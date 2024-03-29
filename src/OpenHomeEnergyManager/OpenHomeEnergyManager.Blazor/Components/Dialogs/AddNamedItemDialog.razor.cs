﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace OpenHomeEnergyManager.Blazor.Components.Dialogs
{
    public partial class AddNamedItemDialog : ComponentBase
    {
        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

        private string _name;
        private bool _isValid;

        void Submit() => MudDialog.Close(DialogResult.Ok(_name));
        void Cancel() => MudDialog.Cancel();
    }
}
