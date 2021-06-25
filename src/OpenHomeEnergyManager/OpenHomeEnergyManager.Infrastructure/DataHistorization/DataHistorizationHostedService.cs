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
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using OpenHomeEnergyManager.Domain.Services.DataHistorizationServices;
using OpenHomeEnergyManager.Domain.Services.VehicleServices;
using OpenHomeEnergyManager.Infrastructure.ChargeModes;
using OpenHomeEnergyManager.Infrastructure.EntityFramework.Repositories;
using OpenHomeEnergyManager.Infrastructure.Modules.Tesla.Authentication;

namespace OpenHomeEnergyManager.Infrastructure.DataHistorization
{
    public class DataHistorizationHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly DataHistorizationService _dataHistorizationService;
        private Timer _timer;

        public DataHistorizationHostedService(ILogger<ChargeModesHostedService> logger, IServiceScopeFactory serviceScopeFactory, DataHistorizationService dataHistorizationService)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _dataHistorizationService = dataHistorizationService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                HistorizeChargePoints(scope);
                HistorizeVehicles(scope);
            }
        }

        private void HistorizeChargePoints(IServiceScope scope)
        {
            ChargePointService chargePointService = scope.ServiceProvider.GetRequiredService<ChargePointService>();
            IChargePointRepository chargePointRepository = scope.ServiceProvider.GetRequiredService<IChargePointRepository>();

            var chargePoints = chargePointRepository.GetAll();

            foreach (var chargePoint in chargePoints.Where(c => c.ModuleId.HasValue))
            {
                var historizationDataset = _dataHistorizationService.GetHistorizationDataset<ChargePointDataset>(chargePoint.Id, TimeSpan.FromSeconds(5));
                historizationDataset.AddDataset(chargePointService.GetCurrentData(chargePoint.ModuleId.Value));
            }
        }

        private void HistorizeVehicles(IServiceScope scope)
        {
            VehicleService vehicleService = scope.ServiceProvider.GetRequiredService<VehicleService>();
            IVehicleRepository vehicleRepository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();

            var vehicles = vehicleRepository.GetAll();

            foreach (var vehicle in vehicles.Where(c => c.ModuleId.HasValue))
            {
                var historizationDataset = _dataHistorizationService.GetHistorizationDataset<VehicleDataset>(vehicle.Id, TimeSpan.FromSeconds(20));
                historizationDataset.AddDataset(vehicleService.GetCurrentData(vehicle.ModuleId.Value));
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
