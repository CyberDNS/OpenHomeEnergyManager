using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Queries;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Shared;

namespace OpenHomeEnergyManager.Blazor.Components.ChargePointManagement
{
    public partial class ChargePointCard : ComponentBase, IDisposable
    {
		[Parameter] public ChargePointDto ChargePoint { get; set; }

		[Inject] private ChargePointsClient _chargePointClient { get; set; }

		public int Power { get; set; }
		public int Current { get; set; }
		public int PhaseCount { get; set; }

		private Timer _timer;

		protected override void OnInitialized()
		{
			Timer _timer = new Timer();
			_timer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
			_timer.Elapsed += TimerElapsed;
			_timer.Enabled = true;
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

		private async Task SetChargeMode(ChargeModesDto chargeMode)
		{
			await _chargePointClient.SelectChargeModeAsync(ChargePoint.Id, new SelectChargeModeDto() { ChargeMode = chargeMode });
			var chargePoints = await _chargePointClient.GetAllAsync();

			ChargePoint = chargePoints.Single(c => c.Id == ChargePoint.Id);

			StateHasChanged();
		}

		private bool IsCurrentChargeMode(ChargeModesDto chargeMode)
		{
			if (ChargePoint?.CurrentChargeMode is null) { return false; }

			return ChargePoint.CurrentChargeMode == chargeMode;
		}

        public void Dispose()
        {
            if (_timer is not null)
            {
				_timer.Stop();
				_timer = null;
            }
        }
    }
}
