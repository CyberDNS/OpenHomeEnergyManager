using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.HomeAssistant
{
    public class TeslaHomeAssistantService : ModuleServiceBase, IHostedModuleService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger _logger;
        private readonly HomeAssistantHttpClient _homeAssistantHttpClient;

        private string _socEntity;
        private string _chargerSwitchEntity;
        private string _chargerIsChargingEntity;


        public TeslaHomeAssistantService(ILogger<TeslaHomeAssistantService> logger, HomeAssistantHttpClient homeAssistantHttpClient)
        {
            _logger = logger;
            _homeAssistantHttpClient = homeAssistantHttpClient;
            RegisterCapability(new StateOfChargeCapability("SOC", "State of Charge"));
            RegisterCapability(new IsChargingCapability("IS_CHARGING", "Is Charging"));
            RegisterCapability(new SetIsChargingCapability("SET_IS_CHARGING", "Set Is Charging", SetIsCharging));
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            _homeAssistantHttpClient._httpClient.BaseAddress = new Uri(settings["Home Assistant URL"]);
            _homeAssistantHttpClient._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings["Long Lived Token"]);

            _socEntity = GetSetting(settings, "State of Charge Entity");
            _chargerSwitchEntity = GetSetting(settings, "Charger Switch Entity");
            _chargerIsChargingEntity = GetSetting(settings, "Charger Sensor Entity");
        }

        private string GetSetting(IDictionary<string, string> settings, string key)
        {
            if (settings.ContainsKey(key)) { return settings[key]; }
            return null;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            if (!string.IsNullOrWhiteSpace(_socEntity))
            {
                string soc = (await _homeAssistantHttpClient.GetState(_socEntity))?.state;
                if (!string.IsNullOrWhiteSpace(soc))
                {
                    try
                    {
                        GetCapability<StateOfChargeCapability>("SOC").Value = Convert.ToDecimal(soc, CultureInfo.InvariantCulture.NumberFormat);
                    }
                    catch (FormatException)
                    {
                        _logger.LogWarning("Converting SOC failed {SocString}", soc);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(_chargerIsChargingEntity))
            {
                string chargingState = (await _homeAssistantHttpClient.GetState(_chargerIsChargingEntity))?.attributes?.charging_state;
                if (!string.IsNullOrWhiteSpace(chargingState))
                {
                    try
                    {
                        GetCapability<IsChargingCapability>("IS_CHARGING").Value = chargingState.Equals("Charging", StringComparison.OrdinalIgnoreCase);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Something went wrong on determining charging state {chargingState}", chargingState);
                    }
                }
            }
        }

        private void SetIsCharging(bool turnOn)
        {
            if (!string.IsNullOrWhiteSpace(_chargerSwitchEntity))
            {
                _homeAssistantHttpClient.CallService($"switch/turn_{(turnOn ? "on" : "off")}", _chargerSwitchEntity).GetAwaiter();
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
