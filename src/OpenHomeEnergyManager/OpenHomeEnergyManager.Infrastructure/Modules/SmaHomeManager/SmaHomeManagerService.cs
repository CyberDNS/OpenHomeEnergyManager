﻿using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Model.ModuleAggregate;
using OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.SmaHomeManager
{
    class SmaHomeManagerService : ModuleServiceBase, IHostedModuleService, IDisposable
    {
        private string _serial;

        private UdpClient _client;
        private readonly ILogger _logger;
        private CancellationTokenSource _tokenSource;

        public SmaHomeManagerService(ILogger<SmaHomeManagerService> logger)
        {
            _logger = logger;

            RegisterCapability(new PowerCapability("IMPORTED_POWER", "Imported Power"));
            RegisterCapability(new PowerCapability("EXPORTED_POWER", "Exported Power"));

        }

        public override void Configure(IDictionary<string, string> settings)
        {
            _serial = settings["Serial Number"];
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenSource = new CancellationTokenSource();

            _client = new UdpClient(9522);
            _client.JoinMulticastGroup(IPAddress.Parse("239.12.255.254"));

            _logger.LogInformation("SMA Home Manager Module joined multicast group");

            Task.Run(() => DoWork(_tokenSource.Token));

            return Task.CompletedTask;
        }

        private async void DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var dataResult = await _client.ReceiveAsync();
                    var data = dataResult.Buffer;

                    if (data[0..3].SequenceEqual(Encoding.ASCII.GetBytes("SMA")))
                    {
                        int dataLength = BitConverter.ToUInt16(data[12..14].Reverse().ToArray()) + 16;
                        uint serial = BitConverter.ToUInt32(data[20..24].Reverse().ToArray());

                        if (serial.ToString().Equals(_serial))
                        {
                            _logger.LogTrace($"DataLength: {dataLength} Serial: {serial}");

                            string message = "";

                            int pointer = 28;
                            while (pointer < dataLength)
                            {
                                var header = DataDecoder.DecodeValueHeader(data[pointer..(pointer + 4)]);
                                pointer += 4;

                                message += $"[{header.Name} {header.Type}] = ";

                                _logger.LogInformation(message);

                                switch (header.Type)
                                {
                                    case DataDecoder.DataType.Current:
                                        uint currentValue = BitConverter.ToUInt32(data[pointer..(pointer + 4)].Reverse().ToArray());
                                        message += $"{currentValue}{Environment.NewLine}";
                                        pointer += 4;

                                        if (header.Name.Equals("pconsume", StringComparison.OrdinalIgnoreCase)) { GetCapability<PowerCapability>("IMPORTED_POWER").Value = (int)currentValue / 10; }
                                        if (header.Name.Equals("psupply", StringComparison.OrdinalIgnoreCase)) { GetCapability<PowerCapability>("EXPORTED_POWER").Value = (int)currentValue / 10; }
                                        break;
                                    case DataDecoder.DataType.Counter:
                                        ulong counterValue = BitConverter.ToUInt64(data[pointer..(pointer + 8)].Reverse().ToArray());
                                        message += $"{counterValue}{Environment.NewLine}";
                                        pointer += 8;
                                        break;
                                    case DataDecoder.DataType.Version:
                                        pointer += 4;
                                        break;
                                    default:
                                        pointer += 4;
                                        break;
                                }
                            }

                            _logger.LogTrace("SmaHomeManager reports Import: {Import} Export: {Export}", GetCapability<PowerCapability>("IMPORTED_POWER").Value, GetCapability<PowerCapability>("EXPORTED_POWER").Value);
                        }
                    }
                }
                catch (SocketException sEx)
                {
                    if (sEx.ErrorCode == 10004) { return; }
                }

                //string dataPowerProvided = $"W,module_name={_module.Name.Replace(" ", @"\ ")},module_id={_module.Id},type=provider  power={CurrentPowerProvided}u";
                //string dataPowerCollected = $"W,module_name={_module.Name.Replace(" ", @"\ ")},module_id={_module.Id},type=collector power={CurrentPowerCollected}u";
                //using (var writeApi = _influxDBClient.GetWriteApi())
                //{
                //    writeApi.WriteRecord(_bucket, _org, WritePrecision.Ns, dataPowerProvided);
                //    writeApi.WriteRecord(_bucket, _org, WritePrecision.Ns, dataPowerCollected);
                //}

                //_logger.LogDebug("SMA Home Manager provided {CurrentPowerProvided} Watt and collected {CurrentPowerCollected} Watt", CurrentPowerProvided, CurrentPowerCollected);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();

            _logger.LogInformation("SMA Home Manager Module stopped");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _tokenSource?.Cancel();
            _client?.Dispose();
        }
    }
}
