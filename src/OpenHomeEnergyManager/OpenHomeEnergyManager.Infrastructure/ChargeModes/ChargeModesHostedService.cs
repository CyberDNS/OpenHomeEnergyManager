using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
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
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CancellationTokenSource _cancellationTokenSource;


        public ChargeModesHostedService(ILogger<ChargeModesHostedService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //return Task.CompletedTask;

            _cancellationTokenSource = new CancellationTokenSource();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IChargePointRepository chargePointRepository = scope.ServiceProvider.GetRequiredService<IChargePointRepository>();

                foreach (var chargePoint in chargePointRepository.GetAll())
                {
                    Task.Run(() => DoWork(chargePoint.Id, _cancellationTokenSource.Token));
                }

            }
            return Task.CompletedTask;
        }

        private async void DoWork(int chargePointId, CancellationToken cancellation)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var chargeModesService = scope.ServiceProvider.GetRequiredService<IChargeModesService>();
                do
                {
                    using (var cpScope = _serviceScopeFactory.CreateScope())
                    {
                        var chargePointRepository = cpScope.ServiceProvider.GetRequiredService<IChargePointRepository>();

                        var chargePoint = await chargePointRepository.FindByIdAsync(chargePointId);

                        _logger.LogInformation("ChargePoint: {name} Mode: {mode}", chargePoint.Name, chargePoint.CurrentChargeMode);

                        await chargeModesService.LoopAsync(chargePoint);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }

                } while (!cancellation.IsCancellationRequested);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
