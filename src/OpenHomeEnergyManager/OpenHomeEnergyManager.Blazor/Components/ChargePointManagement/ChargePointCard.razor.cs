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

        public List<ChartSeries> ChargePointSeries = new List<ChartSeries>()
        {
            new ChartSeries() { Name = "Power", Data = new double[4] { 0, 0, 0, 0 } },
            new ChartSeries() { Name = "11 kW", Data = new double[4] { 11040, 11040, 11040, 11040 } },
            new ChartSeries() { Name = "3.6 kW", Data = new double[4] { 3680, 3680, 3680, 3680 } },
        };

        public List<ChartSeries> VehicleSeries = new List<ChartSeries>()
        {
            new ChartSeries() { Name = "Soc", Data = new double[4] { 0, 0, 0, 0 } },
            new ChartSeries() { Name = "Limit", Data = new double[4] { 0, 0, 0, 0 } },
        };

        protected override void OnInitialized()
        {
            Timer _timer = new Timer();
            _timer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
            _timer.Elapsed += TimerElapsed;
            _timer.Enabled = true;

            _chartOptions.DisableLegend = true;
            _chartOptions.InterpolationOption = InterpolationOption.Straight;
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


            if (_vehicle is not null)
            {
                var currentData = await _chargePointClient.GetNowDataAsync(ChargePoint.Id);
                Power = currentData.Power;
                Current = Convert.ToInt32(Math.Round(currentData.CurrentPhase1, 0));
                PhaseCount = currentData.PhaseCount;

                _vehicleNow = await _vehiclesClient.GetNowDataAsync(_vehicle.Id);

                var historizationChargePointData = await _chargePointClient.GetHistorizationDataAsync(ChargePoint.Id, TimeSpan.FromMinutes(10));
                if (historizationChargePointData.Count() >= 4)
                {
                    List<ChartSeries> newSeries = new List<ChartSeries>()
                    {
                        new ChartSeries() { Name = "Power", Data = historizationChargePointData.Select(d => Convert.ToDouble(d.Power)).ToArray() },
                        new ChartSeries() { Name = "11 kW", Data = new double[4] { 11040, 11040, 11040, 11040 } },
                        new ChartSeries() { Name = "3.6 kW", Data = new double[4] { 3680, 3680, 3680, 3680 } },
                    };
                    ChargePointSeries = newSeries;
                }

                var historizationVehicleData = await _vehiclesClient.GetHistorizationDataAsync(_vehicle.Id, TimeSpan.FromMinutes(10));
                if (historizationVehicleData.Count() >= 4)
                {
                    List<ChartSeries> newSeries = new List<ChartSeries>()
                    {
                        new ChartSeries() { Name = "Soc", Data = historizationVehicleData.Select(d => Convert.ToDouble(d.StateOfCharge)).ToArray() },
                        new ChartSeries() { Name = "Limit", Data = historizationVehicleData.Select(d => Convert.ToDouble(d.ChargeLimit)).ToArray() },
                    };
                    VehicleSeries = newSeries;
                }
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
