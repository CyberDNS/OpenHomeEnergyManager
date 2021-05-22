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
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.OpenWb
{
    public class OpenWbService : IHostedModuleService
    {
        private string _ip;

        private IMqttClient _mqttClient;
        private IMqttClientOptions _mqttOptions;
        private readonly ILogger _logger;

        private HashSet<Capability> _capabilities;
        public IEnumerable<Capability> Capabilities => _capabilities;

        private PowerCapability _capabilityPower = new PowerCapability("POWER", "Power");
        private CurrentCapability _capabilityCurrentConfigured = new CurrentCapability("CURRENT_CONFIGURED", "Current Configured");

        //private readonly InfluxDBClient _influxDBClient;
        //private const string _bucket = "OpenHomeEnergyManager";
        //private const string _org = "InfluxBear";

        public OpenWbService(ILogger<OpenWbService> logger)
        {
            _logger = logger;
        }

        public void Configure(IDictionary<string, string> settings)
        {
            _ip = settings["IP Address"];

            _capabilities = new HashSet<Capability>()
            {
                _capabilityPower,
                _capabilityCurrentConfigured
            };
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
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("openWB/lp/1/AConfigured").Build());
            });

            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                switch (e.ApplicationMessage.Topic)
                {
                    case "openWB/lp/1/W":
                        _capabilityPower.Power = Convert.ToInt32(payload);

                        //string dataPowerConsumed = $"W,module_name={_module.Name.Replace(" ", @"\ ")},module_id={_module.Id},type=consumer  power={CurrentPowerConsumed}u";
                        //using (var writeApi = _influxDBClient.GetWriteApi())
                        //{
                        //    writeApi.WriteRecord(_bucket, _org, WritePrecision.Ns, dataPowerConsumed);
                        //}

                        break;

                    case "openWB/lp/1/AConfigured":
                        _capabilityCurrentConfigured.Current = Convert.ToInt32(payload);

                        break;
                }
            });

            var result = await _mqttClient.ConnectAsync(_mqttOptions, cancellationToken);

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _mqttClient.DisconnectAsync();
        }
    }
}
