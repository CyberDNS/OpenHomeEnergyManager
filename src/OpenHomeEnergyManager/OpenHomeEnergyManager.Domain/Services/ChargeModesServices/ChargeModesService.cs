using Microsoft.Extensions.DependencyInjection;
using OpenHomeEnergyManager.Domain.Model.ChargePointAggregate;
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
        private readonly Dictionary<string, IChargeMode> _chargeModes = new Dictionary<string, IChargeMode>()
        {
            { "STOP", new StopChargeMode() },
            { "DIRECT", new DirectChargeMode() },
        };
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ChargeModesService(IServiceScopeFactory serviceScopeFactory)
        {

            _serviceScopeFactory = serviceScopeFactory;
        }


        public async Task LoopAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IChargePointRepository chargePointRepository = scope.ServiceProvider.GetRequiredService<IChargePointRepository>();
                ChargePointService chargePointService = scope.ServiceProvider.GetRequiredService<ChargePointService>();

                var chargePoints = chargePointRepository.GetAll();

                foreach (var chargePoint in chargePoints)
                {
                    if ((chargePoint.CurrentChargeMode is not null) && (_chargeModes.ContainsKey(chargePoint.CurrentChargeMode)))
                    {
                        await _chargeModes[chargePoint.CurrentChargeMode].LoopAsync(chargePoint, chargePointService);
                    }
                }
            }
        }
    }
}
