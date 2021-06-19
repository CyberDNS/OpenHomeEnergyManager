using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Dtos;

namespace OpenHomeEnergyManager.Blazor.Components.ChargePointManagement
{
    public partial class ChargePointCard : ComponentBase
    {
		[Parameter] public ChargePointDto ChargePoint { get; set; }

		[Inject] private ChargePointClient _chargePointClient { get; set; }

		public int Power { get; set; }
		public int Current { get; set; }
		public int PhaseCount { get; set; }

		protected override void OnInitialized()
		{
			Timer timer = new Timer();
			timer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
			timer.Elapsed += TimerElapsed;
			timer.Enabled = true;
		}

		private void TimerElapsed(object sender, ElapsedEventArgs e)
		{
			InvokeAsync(async () => await UpdateData());
		}

		public async Task UpdateData()
		{
			var currentData = await _chargePointClient.GetNowDataAsync(ChargePoint.Id);
			Power = currentData.Power;
			Current = Convert.ToInt32(Math.Round(currentData.CurrentPhase1, 0));
			PhaseCount = currentData.PhaseCount;

			StateHasChanged();
		}

		private async Task SetChargeMode(string chargeMode)
		{
			await _chargePointClient.SetChargeModeAsync(ChargePoint.Id, chargeMode);
			var chargePoints = await _chargePointClient.GetAllAsync();

			ChargePoint = chargePoints.Single(c => c.Id == ChargePoint.Id);

			StateHasChanged();
		}

		private bool IsCurrentChargeMode(string chargeMode)
		{
			if (ChargePoint?.CurrentChargeMode is null) { return false; }

			return ChargePoint.CurrentChargeMode.Equals(chargeMode, StringComparison.OrdinalIgnoreCase);
		}
	}
}
