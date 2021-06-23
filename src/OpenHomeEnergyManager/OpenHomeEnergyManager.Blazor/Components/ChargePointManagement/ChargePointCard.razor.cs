using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using OpenHomeEnergyManager.Blazor.Components.Dialogs;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Commands;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Queries;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.ChargePoints.Shared;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Images;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles;
using OpenHomeEnergyManager.Blazor.Infrastructure.HttpClients.Vehicles.Queries;

namespace OpenHomeEnergyManager.Blazor.Components.ChargePointManagement
{
    public partial class ChargePointCard : ComponentBase, IDisposable
    {
		[Inject] private IDialogService _dialogService { get; set; }
		[Inject] private ChargePointsClient _chargePointClient { get; set; }
		[Inject] private VehiclesClient _vehiclesClient { get; set; }

		[Parameter] public ChargePointDto ChargePoint { get; set; }


		public int Power { get; set; }
		public int Current { get; set; }
		public int PhaseCount { get; set; }

		private VehicleDatasetDto _vehicleNow { get; set; }

		private Timer _timer;

		private VehicleDto _vehicle;
		private ChartOptions _chartOptions = new ChartOptions();

		public List<ChartSeries> Series = new List<ChartSeries>()
	{
		new ChartSeries() { Name = "Series 1", Data = new double[] { 90, 79, 72, 69, 62, 62, 55, 65, 70 } },
		new ChartSeries() { Name = "Series 2", Data = new double[] { 35, 41, 35, 51, 49, 62, 69, 91, 148 } },
	};
		public string[] XAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

		protected override void OnInitialized()
		{
			Timer _timer = new Timer();
			_timer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
			_timer.Elapsed += TimerElapsed;
			_timer.Enabled = true;

			_chartOptions.DisableLegend = true;
			_chartOptions.InterpolationOption = InterpolationOption.NaturalSpline;
		}

        protected override async Task OnParametersSetAsync()
        {
			await ReloadVehicle();
		}

		private async Task ReloadVehicle()
        {
			if (ChargePoint.VehicleId.HasValue)
			{
				_vehicle = await _vehiclesClient.GetAsync(ChargePoint.VehicleId.Value);
			}
			else
            {
				_vehicle = null;
			}
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

			if (_vehicle is not null)
			{
				_vehicleNow = await _vehiclesClient.GetNowDataAsync(_vehicle.Id);
			}
			else
            {
				_vehicleNow = null;

			}

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

		private async Task AttributeVehicle()
        {
			var dialog = _dialogService.Show<VehicleSelectorDialog>("Select vehicle", new DialogOptions() { MaxWidth = MaxWidth.Large });
			var result = await dialog.Result;

			if (!result.Cancelled)
			{
				await _chargePointClient.AttributeVehicleAsync(ChargePoint.Id, new AttributeVehicleDto() { VehicleId = result.Data.As<int?>() });
				var chargePoints = await _chargePointClient.GetAllAsync();

				ChargePoint = chargePoints.Single(c => c.Id == ChargePoint.Id);
				await ReloadVehicle();

				StateHasChanged();
			}
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
