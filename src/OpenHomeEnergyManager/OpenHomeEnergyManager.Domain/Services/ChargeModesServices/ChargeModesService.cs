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
            var chargeMode = GetChargeMode(chargePoint.CurrentChargeMode);

            if ((chargePoint.ModuleId.HasValue) &&
                (chargePoint.VehicleId.HasValue) &&
                _chargePointService.GetCurrentData(chargePoint.ModuleId.Value).IsPlugged)
            {
                var vehicle = await _vehicleRepository.FindByIdAsync(chargePoint.VehicleId.Value);

                var targetChargeMode = PriorityChargeModes().Select(c =>
                {
                    var cM = GetChargeMode(c);
                    if (cM is null) { return null; }
                    if (cM is IRuleBasedChargeMode rBCM) { if (rBCM.EvaluateRule(chargePoint, vehicle)) { return rBCM; } }
                    return null;
                }).Where(c => c is not null).FirstOrDefault();

                if (targetChargeMode is not null) { chargeMode = targetChargeMode; }

                if (chargeMode is not null) { await chargeMode.LoopAsync(chargePoint, vehicle); }
            }
        }

        private IChargeMode GetChargeMode(ChargeModes chargeMode)
        {
            switch (chargeMode)
            {
                case ChargeModes.Stop:
                    return _serviceProvider.GetRequiredService<StopChargeMode>();
                case ChargeModes.Direct:
                    return _serviceProvider.GetRequiredService<DirectChargeMode>();
                case ChargeModes.Excess:
                    return _serviceProvider.GetRequiredService<ExcessChargeMode>();
                case ChargeModes.DirectTarget:
                    return _serviceProvider.GetRequiredService<DirectTargetChargeMode>();
                case ChargeModes.PlannedTarget:
                    return _serviceProvider.GetRequiredService<PlannedTargetChargeMode>();
                default:
                    return null;
            }
        }

        private IEnumerable<ChargeModes> PriorityChargeModes()
        {
            return new ChargeModes[]
            {
                ChargeModes.DirectTarget,
                ChargeModes.PlannedTarget,
                ChargeModes.NightTarget
            };
        }
    }
}
