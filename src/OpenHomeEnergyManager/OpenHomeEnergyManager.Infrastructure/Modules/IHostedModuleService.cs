using OpenHomeEnergyManager.Domain.Services.ModuleServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules
{
    internal interface IHostedModuleService : IModuleService
    {
        public Task StartAsync(CancellationToken cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken);

    }
}
