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

        private TokenMaterial _tokenMaterial;


        public TeslaService(ILogger<TeslaService> logger, LoginClient loginClient, TeslaClient teslaClient)
        {
            _logger = logger;
            _loginClient = loginClient;
            _teslaClient = teslaClient;
            RegisterCapability(new StateOfChargeCapability("SOC", "State of Charge"));
            RegisterCapability(new IsChargingCapability("IS_CHARGING", "Is Charging"));
            RegisterCapability(new SetIsChargingCapability("SET_IS_CHARGING", "Set Is Charging", SetIsCharging));
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            _email = GetSetting(settings, "E-Mail");
            _password = GetSetting(settings, "Password");
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
            var vehicle = await _teslaClient.GetVehicle();

            if (vehicle.Response.State.Equals("online", StringComparison.OrdinalIgnoreCase))
            {
                var chargeState = await _teslaClient.GetChargeState();
                if (chargeState is not null)
                {
                    GetCapability<StateOfChargeCapability>("SOC").Value = Convert.ToDecimal(chargeState.Response.BatteryLevel, CultureInfo.InvariantCulture.NumberFormat);
                    GetCapability<IsChargingCapability>("IS_CHARGING").Value = chargeState.Response.ChargingState.Equals("Charging", StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        private async void SetIsCharging(bool turnOn)
        {
            if (turnOn)
            {
                await _teslaClient.WakeUp();
                await _teslaClient.StartCharging();
            }
            else
            {
                await _teslaClient.StopCharging();
            }
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
