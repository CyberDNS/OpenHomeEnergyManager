using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos;

namespace OpenHomeEnergyManager.Blazor.Pages.ChargePointManagement.Settings
{
    public partial class ChargePointsSettings : ComponentBase
    {
        [Inject] private ChargePointClient _chargePointClient { get; set; }

        private IList<ChargePointDto> _chargePoints = null;

        protected override async Task OnParametersSetAsync()
        {
            _chargePoints = (await _chargePointClient.GetAllAsync()).ToList();
        }

        private void AddChargePoint()
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            int image = rnd.Next(1, 11);

            _chargePoints.Add(new ChargePointDto() { Image = $"{image}.jpg" });
        }

        public void OnChargePointRemoved(object target, ChargePointDto chargePoint)
        {
            _chargePoints.Remove(chargePoint);
            StateHasChanged();
        }
    }
}
