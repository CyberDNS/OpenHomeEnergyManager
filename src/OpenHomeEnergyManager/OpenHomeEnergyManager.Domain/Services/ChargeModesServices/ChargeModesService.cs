using Microsoft.Extensions.DependencyInjection;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
using OpenHomeEnergyManager.Domain.Model.VehicleAggregate;
using OpenHomeEnergyManager.Domain.Services.ChargePointServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ChargeModesServices
{
    public class ChargeModesService : IChargeModesService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ChargePointService _chargePointService;

        public ChargeModesService(IServiceProvider serviceProvider, IVehicleRepository vehicleRepository, ChargePointService chargePointService)
        {
            _serviceProvider = serviceProvider;
            _vehicleRepository = vehicleRepository;
            _chargePointService = chargePointService;
        }


        public async Task LoopAsync(ChargePoint chargePoint)
        {
            var chargeMode = GetCharge(chargePoint.CurrentChargeMode);
            if ((chargeMode is not null) &&
                (chargePoint.ModuleId.HasValue) &&
                (chargePoint.VehicleId.HasValue) &&
                _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsPlugged)
            {
                var vehicle = await _vehicleRepository.FindByIdAsync(chargePoint.VehicleId.Value);
                await chargeMode.LoopAsync(chargePoint, vehicle);
            }
        }

        private IChargeMode GetCharge(string uniqueIdentifier)
        {
            switch (uniqueIdentifier)
            {
                case "STOP":
                    return _serviceProvider.GetRequiredService<StopChargeMode>();
                case "DIRECT":
                    return _serviceProvider.GetRequiredService<DirectChargeMode>();
                case "EXCESS":
                    return _serviceProvider.GetRequiredService<ExcessChargeMode>();
                default:
                    return null;
            }
        }
    }
}
