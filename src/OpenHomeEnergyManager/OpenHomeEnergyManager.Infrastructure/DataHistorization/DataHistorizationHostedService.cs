using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using OpenHomeEnergyManager.Infrastructure.ChargeModes;
using OpenHomeEnergyManager.Infrastructure.EntityFramework.Repositories;
using OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication;

namespace OpenHomeEnergyManager.Infrastructure.DataHistorization
{
    public class DataHistorizationHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ChargePointService _chargePointService;
        private readonly IChargePointRepository _chargePointRepository;
        private Timer _timer;

        public DataHistorizationHostedService(ILogger<ChargeModesHostedService> logger, 
            ChargePointService chargePointService, IChargePointRepository chargePointRepository)
        {
            _logger = logger;
            _chargePointService = chargePointService;
            _chargePointRepository = chargePointRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            //var chargePoints = _chargePointRepository.GetAll();

            //var data = _chargePointService.GetCurrentData();
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
