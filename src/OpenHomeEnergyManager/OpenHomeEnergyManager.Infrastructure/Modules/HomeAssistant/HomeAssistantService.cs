using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
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
    public class HomeAssistantService : ModuleServiceBase, IHostedModuleService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger _logger;
        private readonly HomeAssistantHttpClient _homeAssistantHttpClient;

        public HomeAssistantService(ILogger<HomeAssistantService> logger, HomeAssistantHttpClient homeAssistantHttpClient)
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
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            string soc = await _homeAssistantHttpClient.GetState("sensor.tessilo_battery_sensor");
            GetCapability<StateOfChargeCapability>("SOC").Value = Convert.ToDecimal(soc, CultureInfo.InvariantCulture.NumberFormat);

            string isCharging = await _homeAssistantHttpClient.GetState("switch.tessilo_charger_switch");
            GetCapability<IsChargingCapability>("IS_CHARGING").Value = isCharging.Equals("on", StringComparison.OrdinalIgnoreCase);
        }

        private void SetIsCharging(bool turnOn)
        {
            _homeAssistantHttpClient.CallService($"switch/turn_{(turnOn ? "on" : "off")}", "switch.tessilo_charger_switch").GetAwaiter();
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
