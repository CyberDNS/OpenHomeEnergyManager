using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Queries;

namespace OpenHomeEnergyManager.Blazor.Pages
{
    public partial class Dashboard : ComponentBase
    {
        [Inject] private ChargePointsClient _chargePointClient { get; set; }

        private IList<ChargePointDto> _chargePoints = null;

        protected override async Task OnParametersSetAsync()
        {
            _chargePoints = (await _chargePointClient.GetAllAsync()).ToList();
        }
    }
}
