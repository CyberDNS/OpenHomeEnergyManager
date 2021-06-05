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
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.OpenWb
{
    public class OpenWbService : ModuleServiceBase, IHostedModuleService
    {
        private string _ip;

        private IMqttClient _mqttClient;
        private IMqttClientOptions _mqttOptions;
        private readonly ILogger _logger;

        public OpenWbService(ILogger<OpenWbService> logger)
        {
            _logger = logger;

            RegisterCapability(new PowerCapability("POWER", "Power"));

            RegisterCapability(new CurrentCapability("CURRENT_PHASE_1", "Current Phase 1"));
            RegisterCapability(new CurrentCapability("CURRENT_PHASE_2", "Current Phase 2"));
            RegisterCapability(new CurrentCapability("CURRENT_PHASE_3", "Current Phase 3"));

            RegisterCapability(new VoltageCapability("VOLTAGE_PHASE_1", "Voltage Phase 1"));
            RegisterCapability(new VoltageCapability("VOLTAGE_PHASE_2", "Voltage Phase 2"));
            RegisterCapability(new VoltageCapability("VOLTAGE_PHASE_3", "Voltage Phase 3"));

            RegisterCapability(new PhaseCountCapability("PHASE_COUNT", "Phase Count"));

            RegisterCapability(new IsChargingCapability("IS_CHARGING", "Is Charging"));
            RegisterCapability(new IsPluggedCapability("IS_PLUGGED", "Is Plugged"));

            RegisterCapability(new StateOfChargeCapability("SOC", "State of Charge"));

            RegisterCapability(new SetCurrentCapability("SET_CURRENT", "Set Current", SetCurrent));
            RegisterCapability(new SetPhaseCountCapability("SET_PHASE_COUNT", "Set Phase Count", SetPhaseCount));
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            _ip = settings["IP Address"];
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(_ip)
                .Build();

            _mqttClient.UseDisconnectedHandler(async e =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await _mqttClient.ConnectAsync(_mqttOptions, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Reconnection to MQTT failed");
                }
            });

            _mqttClient.UseConnectedHandler(async e =>
            {
                _logger.LogInformation("Connected to MQTT server");
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/W").Build());
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/APhase1").Build());
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/APhase2").Build());
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/APhase3").Build());
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/VPhase1").Build());
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/VPhase2").Build());
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/VPhase3").Build());
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/boolPlugStat").Build());
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/%Soc").Build());
            });

            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                switch (e.ApplicationMessage.Topic)
                {
                    case "openWB/lp/1/W":
                        GetCapability<PowerCapability>("POWER").Value = Convert.ToInt32(payload);
                        break;
                    case "openWB/lp/1/APhase1":
                        GetCapability<CurrentCapability>("CURRENT_PHASE_1").Value = Convert.ToDecimal(payload, CultureInfo.InvariantCulture.NumberFormat);
                        break;
                    case "openWB/lp/1/APhase2":
                        GetCapability<CurrentCapability>("CURRENT_PHASE_2").Value = Convert.ToDecimal(payload, CultureInfo.InvariantCulture.NumberFormat);
                        break;
                    case "openWB/lp/1/APhase3":
                        GetCapability<CurrentCapability>("CURRENT_PHASE_3").Value = Convert.ToDecimal(payload, CultureInfo.InvariantCulture.NumberFormat);
                        break;
                    case "openWB/lp/1/VPhase1":
                        GetCapability<VoltageCapability>("VOLTAGE_PHASE_1").Value = Convert.ToDecimal(payload, CultureInfo.InvariantCulture.NumberFormat);
                        break;
                    case "openWB/lp/1/VPhase2":
                        GetCapability<VoltageCapability>("VOLTAGE_PHASE_2").Value = Convert.ToDecimal(payload, CultureInfo.InvariantCulture.NumberFormat);
                        break;
                    case "openWB/lp/1/VPhase3":
                        GetCapability<VoltageCapability>("VOLTAGE_PHASE_3").Value = Convert.ToDecimal(payload, CultureInfo.InvariantCulture.NumberFormat);
                        break;
                    case "openWB/lp/1/boolPlugStat":
                        GetCapability<IsPluggedCapability>("IS_PLUGGED").Value = payload.Equals("1") ? true : false;
                        break;
                    case "openWB/lp/1/%Soc":
                        GetCapability<StateOfChargeCapability>("SOC").Value = Convert.ToDecimal(payload, CultureInfo.InvariantCulture.NumberFormat);
                        break;
                }

                int phaseCount = 0;
                if (GetCapability<CurrentCapability>("CURRENT_PHASE_1").Value > 3) { phaseCount = 1; }
                if (GetCapability<CurrentCapability>("CURRENT_PHASE_2").Value > 3) { phaseCount = 2; }
                if (GetCapability<CurrentCapability>("CURRENT_PHASE_3").Value > 3) { phaseCount = 3; }
                GetCapability<PhaseCountCapability>("PHASE_COUNT").Value = phaseCount;

                if (phaseCount > 0) { GetCapability<IsChargingCapability>("IS_CHARGING").Value = true; }
                else { GetCapability<IsChargingCapability>("IS_CHARGING").Value = false; }
            });

            var result = await _mqttClient.ConnectAsync(_mqttOptions, cancellationToken);

        }

        private void SetCurrent(int current)
        {
            if (_mqttClient.IsConnected)
            {
                _logger.LogInformation("SetCurrent to {current}", current);
                _mqttClient.PublishAsync("openWB/set/isss/Current", $"{current}").GetAwaiter().GetResult();
            }
        }

        private void SetPhaseCount(int phases)
        {
            if (_mqttClient.IsConnected)
            {
                _logger.LogInformation("SetPhaseCount to {phases}", phases);
                _mqttClient.PublishAsync("openWB/set/isss/U1p3p", $"{phases}").GetAwaiter().GetResult();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _mqttClient.DisconnectAsync();
        }
    }
}
