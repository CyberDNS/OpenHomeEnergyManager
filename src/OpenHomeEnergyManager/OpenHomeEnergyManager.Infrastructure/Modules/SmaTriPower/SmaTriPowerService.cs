using FluentModbus;
using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.SmaTriPower
{
    public class SmaTriPowerService : ModuleServiceBase, IHostedModuleService, IDisposable
    {
        private string _ip;

        private Timer _timer;
        private ModbusTcpClient _client;
        private readonly ILogger _logger;

        public int CurrentPowerProvided { get; private set; }

        public SmaTriPowerService(ILogger<SmaTriPowerService> logger)
        {
            _logger = logger;
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            _ip = settings["IP Address"];
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _client = new ModbusTcpClient();
                _client.Connect(IPAddress.Parse(_ip));

                _logger.LogInformation("SMA Tripower Module connected on {Ip}", _ip);
            }
            catch (Exception ex)
            {
                return Task.CompletedTask;
            }

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                byte unitIdentifier = 3;

                var powerSpan = _client.ReadHoldingRegisters(unitIdentifier, 30775, 2);
                powerSpan.Reverse();

                int power = BitConverter.ToInt32(powerSpan);

                CurrentPowerProvided = Math.Max(power, 0);

                //string data = $"W,module_name={_module.Name.Replace(" ", @"\ ")},module_id={_module.Id},type=provider power={CurrentPowerProvided}u";
                //using (var writeApi = _influxDBClient.GetWriteApi())
                //{
                //    writeApi.WriteRecord(_bucket, _org, WritePrecision.Ns, data);
                //}

                _logger.LogDebug("SMA TriPower provides {CurrentPowerProvided} Watt", CurrentPowerProvided);
            } catch (Exception ex)
            {

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _client.Disconnect();

            _logger.LogInformation("SMA Tripower Module disconnected");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
