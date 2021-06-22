using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Blazor.Theme
{
    public class DefaultTheme : MudTheme
    {
        public DefaultTheme()
        {
            Palette = new Palette()
            {
                AppbarBackground = Colors.DeepOrange.Default,
                Primary = Colors.DeepOrange.Default,
                Secondary = Colors.Blue.Default,
                TextPrimary = Colors.Grey.Darken2,
                TextSecondary = Colors.Grey.Darken4
            };
        }
    }
}
