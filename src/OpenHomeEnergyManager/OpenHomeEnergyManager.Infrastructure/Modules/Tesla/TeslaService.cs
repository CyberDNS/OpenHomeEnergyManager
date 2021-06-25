using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using OpenHomeEnergyManager.Infrastructure.Modules.HomeAssistant;
using OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication;

namespace OpenHomeEnergyManager.Infrastructure.Modules.Tesla
{
    public class TeslaService : ModuleServiceBase, IHostedModuleService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger _logger;
        private readonly LoginClient _loginClient;
        private readonly TeslaClient _teslaClient;
        private string _email;
        private string _password;
        private string _vehicleId;

        private bool _connectedWallboxIsCharging = false;

        private TokenMaterial _tokenMaterial;


        public TeslaService(ILogger<TeslaService> logger, LoginClient loginClient, TeslaClient teslaClient)
        {
            _logger = logger;
            _loginClient = loginClient;
            _teslaClient = teslaClient;
            RegisterCapability(new StateOfChargeCapability("SOC", "State of Charge"));
            RegisterCapability(new ChargeLimitCapability("CHARGE_LIMIT", "Charge Limit"));
            RegisterCapability(new IsChargingCapability("IS_CHARGING", "Is Charging"));
            RegisterCapability(new IsChargedToChargeLimitCapability("IS_CHARGED_TO_CHARGE_LIMIT", "Is charged-up to the limit"));

            RegisterCapability(new SetIsChargingCapability("SET_IS_CHARGING", "Set Is Charging", SetIsCharging));

            RegisterCapability(new SetIsChargingCapability("SET_CONNECTED_WALLBOX_IS_CHARGING", "Set connected Wallbox is charging", SetConnectedWallboxIsCharging));
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            _email = GetSetting(settings, "E-Mail");
            _password = GetSetting(settings, "Password");
            _vehicleId = GetSetting(settings, "Vehicle ID");
        }

        private string GetSetting(IDictionary<string, string> settings, string key)
        {
            if (settings.ContainsKey(key)) { return settings[key]; }
            return null;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenMaterial = await _loginClient.Login(_email, _password);
            _teslaClient.SetAccessToken(_tokenMaterial.AccessToken);

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));

        }

        private async void DoWork(object state)
        {
            try
            {
                if (!_connectedWallboxIsCharging) { return; }

                var vehicle = await _teslaClient.GetVehicle(_vehicleId);

                if (vehicle.Response.State.Equals("online", StringComparison.OrdinalIgnoreCase))
                {
                    var chargeState = await _teslaClient.GetChargeState(_vehicleId);
                    if (chargeState is not null)
                    {
                        GetCapability<StateOfChargeCapability>("SOC").Value = Convert.ToDecimal(chargeState.Response.BatteryLevel, CultureInfo.InvariantCulture.NumberFormat);
                        GetCapability<ChargeLimitCapability>("CHARGE_LIMIT").Value = Convert.ToDecimal(chargeState.Response.ChargeLimitSoc, CultureInfo.InvariantCulture.NumberFormat);

                        GetCapability<IsChargingCapability>("IS_CHARGING").Value = chargeState.Response.ChargingState.Equals("Charging", StringComparison.OrdinalIgnoreCase);
                        GetCapability<IsChargedToChargeLimitCapability>("IS_CHARGED_TO_CHARGE_LIMIT").Value = chargeState.Response.ChargingState.Equals("Complete", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception!");
            }
        }

        private async void SetIsCharging(bool turnOn)
        {
            try
            {
                if (turnOn)
                {
                    await _teslaClient.WakeUp(_vehicleId);
                    await _teslaClient.StartCharging(_vehicleId);
                }
                else
                {
                    await _teslaClient.StopCharging(_vehicleId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception!");
            }
        }

        private void SetConnectedWallboxIsCharging(bool isCharging)
        {
            _connectedWallboxIsCharging = isCharging;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
