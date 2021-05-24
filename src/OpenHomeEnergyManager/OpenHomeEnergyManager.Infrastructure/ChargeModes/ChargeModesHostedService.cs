using Microsoft.Extensions.Hosting;
using OpenHomeEnergyManager.Domain.Services.ChargeModesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.ChargeModes
{
    public class ChargeModesHostedService : IHostedService
    {
        private readonly IChargeModesService _chargeModesService;
        private CancellationTokenSource _cancellationTokenSource;


        public ChargeModesHostedService(IChargeModesService chargeModesService)
        {
            _chargeModesService = chargeModesService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            DoWork(_cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        private async void DoWork(CancellationToken cancellation)
        {
            do
            {
                await _chargeModesService.LoopAsync();
                await Task.Delay(TimeSpan.FromSeconds(5));

            } while (!cancellation.IsCancellationRequested);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
